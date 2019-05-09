using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Vhc.CoreUi.Abstractions;
using Vhc.Optimus.Core.Abstractions;
using Vhc.Optimus.Core.Services;
using Vhc.Optimus.Core.Utils;
using Vhc.Optimus.ReferenceImpl;
using Vhc.Optimus.Services;

namespace Vhc.Optimus.ReferenceApp
{
    internal class Startup : IStartup
    {
        public void Configure(IConfigurationBuilder config)
        {
            config.AddJsonFile("appsettings.json");
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            // services.AddLogging(builder => builder.AddConfiguration(config).AddConsole());
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole();
            services.AddSingleton(loggerFactory);
            services.AddLogging();

            services.AddReferenceServices(config);
        }

        public void Start(IAppHost app) => StartAsync(app).GetAwaiter().GetResult();

        public async Task StartAsync(IAppHost app)
        {
            var criteria = app.Services.GetService<ICriteria>();
            var logger = app.Services.GetService<ILogger<Startup>>();
            logger.LogInformation($"{ Constants.AppName} - v{ Constants.AppVersion }");
            logger.LogInformation($"Event : {criteria}");

            var runner = app.Services.GetService<JobRunner>();
            var cts = new CancellationTokenSource();
            var result = await runner.ExecuteAll(criteria, cts.Token);

        }
    }
}