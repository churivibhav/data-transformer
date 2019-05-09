using System;
using System.Collections.Generic;
using System.Text;

namespace Vhc.Optimus.Core.Abstractions
{
    public interface ICriteria
    {
        bool RunAll { get; set; }
        string[] RunJobs { get; set; }
        string RunFolder { get; set; }
        bool SuspendWarehouses { get; set; }
        IDictionary<string, IDictionary<string, string>> JobVariables { get; set; }

        bool IsProper();
    }
}
