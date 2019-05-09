using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vhc.Optimus.Core.Abstractions;
using Vhc.Optimus.Core.Models;
using Vhc.Optimus.Core.Services;
using Vhc.Optimus.Services;

namespace Vhc.Optimus.ReferenceImpl
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
            System.Net.ServicePointManager.DefaultConnectionLimit = 50;
            return services.AddTransient<INotificationService, ConsoleNotificationService>()
            .AddSingleton<IExecutionContext, ContinuousExecutionContext>()
            .AddTransient<ITextFileProvider, PlainTextFileProvider>()
            .AddJobLoader(unitFactory)
            .AddSingleton<IConnectionProvider, SqliteConnectionProvider>()
            .AddJobRunner(options => options.AddSqliteDatabase(config));
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