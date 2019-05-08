using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vhc.DataTransformer.Core.Abstractions;
using Vhc.DataTransformer.Core.Models;
using Vhc.DataTransformer.Core.Services;
using Vhc.DataTransformer.Services;

namespace Vhc.DataTransformer.ReferenceImpl
{
    public static class ServiceExtensions
    {
        private static readonly Func<string, IJobUnit> unitFactory = (unitType) => unitType switch
        {
            "Python" => new PythonJobUnit(),
            "Sql" => new SqlJobUnit(),
            "Job" => new Job() as IJobUnit,
            _ => throw new Exception("Invalid unit type!")
        };

        public static IServiceCollection AddReferenceServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<INotificationService, ConsoleNotificationService>();
            services.AddSingleton<IExecutionContext, ContinuousExecutionContext>();
            services.AddTransient<ITextFileProvider, PlainTextFileProvider>();
            services.AddJobLoader(unitFactory);
            services.AddSingleton<IConnectionProvider, SqliteConnectionProvider>();
            services.AddJobRunner(options => options.AddSqliteDatabase(config));
            System.Net.ServicePointManager.DefaultConnectionLimit = 50;
            return services;
        }

        private static void AddJobLoader(this IServiceCollection services, Func<string, IJobUnit> unitFactoryFunction)
        {
            services.AddTransient<IJobLoader, JobLoader>(provider => new JobLoader(
                      provider.GetService<IConfiguration>(),
                      provider.GetService<ITextFileProvider>(),
                      provider.GetService<ILogger<JobLoader>>(),
                      unitFactoryFunction));
        }

        public static IServiceCollection AddJobRunner(this IServiceCollection services, Action<JobRunnerOptions> optionsModifier)
        {
            services.AddTransient<JobRunner>(provider =>
            {
                var jobRunner = new JobRunner(
                    provider.GetService<IConnectionProvider>(),
                    provider.GetService<ILogger<JobRunner>>(),
                    provider.GetService<IJobLoader>(),
                    provider.GetService<INotificationService>(),
                    provider.GetService<IExecutionContext>()
                    );
                return jobRunner.WithOptions(optionsModifier);
            });
            return services;
        }

        public static void AddSqliteDatabase(this JobRunnerOptions options, IConfiguration config)
        {
            var connectionSet = new HashSet<IDatabaseConnection>();
            var snowflake = config.GetSection("Databases").GetChildren();
            foreach (var connConfig in snowflake)
            {
                connectionSet.Add(new DatabaseConnection
                {
                    Name = connConfig["name"],
                    ConnectionString = connConfig["connectionString"]
                });
            }
            options.DatabaseConnections = connectionSet;
            options.ConcurrentNumberOfJobs = Convert.ToInt32(config["Transformations:ConcurrentNumberOfJobs"]);
        }
    }
}