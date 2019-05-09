using Microsoft.Extensions.Logging;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;
using System.Data;
using Vhc.Optimus.Core.Abstractions;
using Vhc.Optimus.Core.Services;

namespace Vhc.Optimus.Core.Abstractions
{
    public interface IResources
    {
        IExecutionContext Context { get; set; }
        ILogger Logger { get; set; }
        IJobLoader JobLoader { get; set; }
        IDictionary<string, string> Variables { get; set; }
        IDbTransaction DbTransaction { get; set; }
        ScriptEngine ScriptEngine { get; set; }
        ILogFormatter LogFormatter { get; set; }
        bool AbortOnFailure { get; set; }
    }
}