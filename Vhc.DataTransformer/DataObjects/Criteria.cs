using System.Collections.Generic;
using Vhc.DataTransformer.Core.Abstractions;

namespace Vhc.DataTransformer.DataObjects
{
    public class Criteria : ICriteria
    {
        public bool RunAll { get; set; } = false;
        public string[] RunJobs { get; set; } = new string[] { };
        public string RunFolder { get; set; } = default;
        public bool SuspendWarehouses { get; set; } = true;
        public IDictionary<string, IDictionary<string, string>> JobVariables { get; set; } = new Dictionary<string, IDictionary<string, string>>();

        public bool IsProper()
            => RunAll || !string.IsNullOrWhiteSpace(RunFolder) || RunJobs.Length > 0;

        public override string ToString()
            => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
