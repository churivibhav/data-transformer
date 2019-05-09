using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Scripting.Hosting;
using Vhc.Optimus.Core.Abstractions;
using Vhc.Optimus.Core.Services;

namespace Vhc.Optimus.Core.Models
{
    public class Resources : IResources
    {
        public IJobLoader JobLoader { get; set; }
        public IDbTransaction DbTransaction { get; set; }
        public ScriptEngine ScriptEngine { get; set; }
        public IDictionary<string, string> Variables { get; set; }
        public ILogger Logger { get; set; }
        public ILogFormatter LogFormatter { get; set; }
        public bool AbortOnFailure { get; set; } = true;
        public IExecutionContext Context { get; set; }
    }
}