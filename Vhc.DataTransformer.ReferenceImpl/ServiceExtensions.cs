//using System;
//using System.Collections.Generic;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;

//namespace Vhc.DataTransformer.ReferenceImpl
//{
//    public static class ServiceExtensions
//    {
//        public static LambdaLoggerOptions AddLambdaLoggerOptions(this IServiceCollection serviceCollection)
//            => new LambdaLoggerOptions
//            {
//                IncludeCategory = false,
//                IncludeLogLevel = true,
//                IncludeNewline = true,
//                Filter = (category, logLevel) =>
//                {
//                    // For some categories, only log events with minimum LogLevel
//                    if (string.Equals(category, "Default", StringComparison.Ordinal))
//                    {
//                        return logLevel >= LogLevel.Debug;
//                    }
//                    if (string.Equals(category, "Microsoft", StringComparison.Ordinal))
//                    {
//                        return logLevel >= LogLevel.Information;
//                    }
//                    else return logLevel >= LogLevel.Debug;
//                    // return true;
//                }
//            };

//        public static IServiceCollection AddSnsNotifications(this IServiceCollection services, IConfiguration config)
//            => services.AddTransient<INotificationService, AmazonSimpleNotificationService>(provider => new AmazonSimpleNotificationService(
//                provider.GetService<ILogger<AmazonSimpleNotificationService>>(),
//                provider.GetService<IAmazonSimpleNotificationService>(),
//                new NotificationOptions
//                {
//                    TopicArn = config["Notifications:SNS:TopicArn"]
//                }));

//        public static IServiceCollection AddEmailNotifications(this IServiceCollection services, IConfiguration config)
//        {
//            services.AddTransient<INotificationService, EmailNotificationService>(provider =>
//            {
//                return new EmailNotificationService(
//                    provider.GetService<ILogger<EmailNotificationService>>(),
//                    provider.GetService<IAmazonSimpleEmailService>(),
//                    new EmailOptions
//                    {
//                        From = Environment.GetEnvironmentVariable("EMAIL_FROM"),
//                        To = Environment.GetEnvironmentVariable("EMAIL_TO").Split(","),
//                        SubjectFailure = config["Notifications:Email:SubjectFailure"]
//                    });
//            });
//            return services;
//        }

//        public static IServiceCollection SetEnvironmentFromConfiguration(this IServiceCollection services, IConfiguration config)
//        {
//            /* Code to set AWS Credentials for local tests */
//            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", config["AWS:AccessKey"]);
//            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", config["AWS:SecretKey"]);
//            Environment.SetEnvironmentVariable("AWS_REGION", config["AWS:Region"]);

//            Environment.SetEnvironmentVariable("EMAIL_FROM", config["Notifications:Email:From"]);
//            Environment.SetEnvironmentVariable("EMAIL_TO", config["Notifications:Email:To"]);
//            return services;
//        }

//        public static IServiceCollection AddJobRunner(this IServiceCollection services, Action<JobRunnerOptions> optionsModifier)
//        {
//            services.AddTransient<JobRunner>(provider =>
//            {
//                var jobRunner = new JobRunner(
//                    provider.GetService<IConnectionProvider>(),
//                    provider.GetService<ILogger<JobRunner>>(),
//                    provider.GetService<IJobLoader>(),
//                    provider.GetService<INotificationService>(),
//                    provider.GetService<IExecutionContext>()
//                    );
//                return jobRunner.WithOptions(optionsModifier);
//            });
//            return services;
//        }

//        public static IServiceCollection AddAwsServices(this IServiceCollection services, IConfiguration configuration)
//        {
//            var awsOptions = configuration.GetAWSOptions();
//            awsOptions.Credentials = new Amazon.Runtime.EnvironmentVariablesAWSCredentials();
//            services.AddDefaultAWSOptions(awsOptions);
//            services.AddAWSService<Amazon.S3.IAmazonS3>();
//            // services.AddAWSService<Amazon.SimpleEmail.IAmazonSimpleEmailService>();
//            services.AddAWSService<Amazon.SimpleNotificationService.IAmazonSimpleNotificationService>();
//            return services;
//        }

//        public static void AddSnowflakeDatabase(this JobRunnerOptions options, IConfiguration config)
//        {
//            var connectionSet = new HashSet<IDatabaseConnection>();
//            var snowflake = config.GetSection("Snowflake").GetChildren();
//            foreach (var connConfig in snowflake)
//            {
//                connectionSet.Add(new DatabaseConnection
//                {
//                    Name = connConfig["name"],
//                    ConnectionString = connConfig["db"] == null ? string.Format(Constants.ConnectionStringFormatInternal, connConfig["account"],
//                        connConfig["host"],
//                        connConfig["user"],
//                        connConfig["password"])
//                    : string.Format(Constants.ConnectionStringFormatDefault,
//                        connConfig["account"],
//                        connConfig["host"],
//                        connConfig["user"],
//                        connConfig["password"],
//                        connConfig["role"],
//                        connConfig["db"],
//                        connConfig["schema"],
//                        Constants.WAREHOUSE
//                        )
//                });
//            }
//            options.DatabaseConnections = connectionSet;
//            options.ConcurrentNumberOfJobs = Convert.ToInt32(config["Transformations:ConcurrentNumberOfJobs"]);
//        }
//    }
//}