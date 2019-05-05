using System.Collections.Generic;
using Vhc.DataTransformer.Core.Abstractions;

namespace Vhc.DataTransformer.Core.Services
{
    public class JobRunnerOptions
    {
        public ISet<IDatabaseConnection> DatabaseConnections { get; set; }
        public int ConcurrentNumberOfJobs { get; set; } = -1;
    }
}