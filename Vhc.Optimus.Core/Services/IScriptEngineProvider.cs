using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vhc.Optimus.Core.Services
{
    public interface IScriptEngineProvider
    {
        ScriptEngine ScriptEngine { get; }
    }
}
