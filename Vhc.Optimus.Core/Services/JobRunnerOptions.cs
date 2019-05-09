using System.Collections.Generic;
using Vhc.Optimus.Core.Abstractions;

namespace Vhc.Optimus.Core.Services
{
    public class JobRunnerOptions
    {
        public ISet<IDatabaseConnection> DatabaseConnections { get; set; }
        public int ConcurrentNumberOfJobs { get; set; } = -1;
    }
}