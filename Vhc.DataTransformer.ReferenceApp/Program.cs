using Microsoft.Extensions.DependencyInjection;
using System;
using Vhc.CoreUi;
using Vhc.DataTransformer.Core.Abstractions;
using Vhc.DataTransformer.DataObjects;

namespace Vhc.DataTransformer.ReferenceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var criteria = new Criteria
            {
                RunJobs = new[] { "ABC" }
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
