using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vhc.Optimus.Core.Abstractions;
using Vhc.Optimus.Core.Models;
using Vhc.Optimus.Core.Utils;

namespace Vhc.Optimus.Core.Services
{
    public class JobRunner
    {
        private JobRunnerOptions options;
        private readonly IJobLoader jobLoader;
        private readonly INotificationService notificationService;
        private readonly IExecutionContext context;
        private readonly IConnectionProvider connectionProvider;
        private readonly IScriptEngineProvider scriptEngineProvider;
        private readonly ILogger<JobRunner> logger;

        public JobRunner(IConnectionProvider connectionProvider, IScriptEngineProvider scriptEngineProvider, ILogger<JobRunner> logger, IJobLoader jobLoader, INotificationService notificationService, IExecutionContext context)
        {
            this.connectionProvider = connectionProvider;
            this.scriptEngineProvider = scriptEngineProvider;
            this.logger = logger;
            this.jobLoader = jobLoader;
            this.notificationService = notificationService;
            this.context = context;
            options = new JobRunnerOptions();
        }

        public JobRunner WithOptions(Action<JobRunnerOptions> optionsModifier)
        {
            optionsModifier?.Invoke(options);
            return this;
        }

        public async Task<IResult> ExecuteAll(ICriteria criteria, CancellationToken token)
        {
            logger.LogDebug("Starting job runner...");
            try
            {
                if (!criteria.IsProper())
                {
                    throw new Exception("Invalid Criteria! Aborting...");
                }
                var scriptEngine = scriptEngineProvider.ScriptEngine;
                var activeParentJobs = await jobLoader.LoadAllParentJobsAsync(criteria);
                var eligibleJobs = activeParentJobs.OrderBy(job => job.Priority);
                logger.LogInformation($"The following jobs will run : {string.Join(",", eligibleJobs.Select(job => job.Name))}");
                Parallel.ForEach(eligibleJobs,
                    new ParallelOptions { MaxDegreeOfParallelism = options.ConcurrentNumberOfJobs },
                    job =>
                    {
                        ConnectAndExecute(job, scriptEngine, abortOnFailure: true);
                    });
                logger.LogInformation($"COMPLETED {eligibleJobs.Count()} jobs.");
                logger.LogInformation($"COMPLETED lambda execution.");
            }
            catch (Exception ex)
            {
                var exceptionBuilder = new System.Text.StringBuilder();
                exceptionBuilder.AppendLine($"Event: {criteria}");
                exceptionBuilder.AppendLine($"Time Remaining: {context.RemainingTime}");
                if (ex is AggregateException ae)
                {
                    foreach (var exception in ae.InnerExceptions)
                    {
                        exceptionBuilder.AppendLine(exception.GetType().Name + " : " + exception.Message);
                    }
                }
                else
                {
                    exceptionBuilder.AppendLine(ex.Message);
                }

                string detailedExceptionMessage = exceptionBuilder.ToString();
                logger.LogError(detailedExceptionMessage);
                await notificationService.NotifyFailureAsync(detailedExceptionMessage);
                return new Result
                {
                    Success = false,
                    Message = detailedExceptionMessage
                };
            }
            return new Result
            {
                Success = true
            };
        }

        private void ConnectAndExecute(IJob job, Microsoft.Scripting.Hosting.ScriptEngine scriptEngine, bool abortOnFailure)
        {
            using (IDbConnection dbConnection = connectionProvider.NewConnection)
            {
                var connectionTimer = new PerformanceTimer().Start();
                SetupAndOpenDatabaseConnection(job, dbConnection);
                var formatter = new LogFormatter().ForJob(job.Name, job.ConnectionName);
                logger.LogInformation(formatter.TextForJob(connectionTimer.StopAndGetElapsedTime().ToFormattedString(), context.RemainingTime.ToFormattedString(), "Opened Database Connection"));

                using (IDbTransaction transaction = dbConnection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var jobTimer = new PerformanceTimer().Start();
                    try
                    {
                        var result = job.Execute(new Resources
                        {
                            JobLoader = jobLoader,
                            ScriptEngine = scriptEngine,
                            DbTransaction = transaction,
                            Logger = logger,
                            LogFormatter = formatter,
                            AbortOnFailure = abortOnFailure,
                            Context = context
                        });
                        var elapsedTime = jobTimer.StopAndGetElapsedTime();
                        if (result.Success)
                        {
                            logger.LogInformation(formatter.TextForJob(elapsedTime.ToFormattedString(), context.RemainingTime.ToFormattedString(), result.Message));
                            transaction.Commit();
                        }
                        else
                        {
                            throw new Exception(result.Message);
                        }
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        var elapsedTime = jobTimer.StopAndGetElapsedTime();
                        logger.LogWarning(formatter.TextForJob(elapsedTime.ToFormattedString(), string.Empty, "EXCEPTION - Changes rolled back"));
                        dbConnection.Close();
                        logger.LogError(e.ToString());
                        throw e;
                    }
                }
                dbConnection.Close();
                logger.LogInformation(formatter.TextForJob(string.Empty, string.Empty, "Closed Database Connection"));
            }
        }


        private void SetupAndOpenDatabaseConnection(IJob job, IDbConnection dbConnection)
        {
            dbConnection.ConnectionString = options.DatabaseConnections.Where(c => c.Name == job.ConnectionName).Select(c => c.ConnectionString).First();
            dbConnection.Open();
        }
    }
}