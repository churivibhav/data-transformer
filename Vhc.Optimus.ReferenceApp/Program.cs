using Microsoft.Extensions.DependencyInjection;
using System;
using Vhc.CoreUi;
using Vhc.Optimus.Core.Abstractions;
using Vhc.Optimus.DataObjects;

namespace Vhc.Optimus.ReferenceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var criteria = new Criteria
            {
                RunJobs = new[] { "DemoJob" }
            };
            new AppHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ICriteria>(criteria);
                })
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
