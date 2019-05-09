using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Vhc.Optimus.Core.Abstractions;
using Vhc.Optimus.Core.Services;

namespace Vhc.Optimus.Services
{
    public static class OptimusExtensions
    {
        public static IServiceCollection AddJobLoader(this IServiceCollection services, Func<string, IJobUnit> unitFactoryFunction) 
            => services.AddTransient<IJobLoader, JobLoader>(provider => new JobLoader(
                provider.GetService<IConfiguration>(),
                provider.GetService<ITextFileProvider>(),
                provider.GetService<ILogger<JobLoader>>(),
                unitFactoryFunction));

        public static IServiceCollection AddJobRunner(this IServiceCollection services, Action<JobRunnerOptions> optionsModifier) 
            => services.AddTransient(provider =>
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
    }
}
