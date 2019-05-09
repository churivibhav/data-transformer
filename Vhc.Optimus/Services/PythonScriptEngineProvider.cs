using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Scripting.Hosting;
using Vhc.Optimus.Core.Services;

namespace Vhc.Optimus.Services
{
    public class PythonScriptEngineProvider : IScriptEngineProvider
    {
        private readonly ILogger<PythonScriptEngineProvider> logger;

        public ScriptEngine ScriptEngine { get; }

        public PythonScriptEngineProvider(ILogger<PythonScriptEngineProvider> logger)
        {
            ScriptEngine = CreateScriptEngine();
            this.logger = logger;
        }

        private ScriptEngine CreateScriptEngine()
        {
            var engine = IronPython.Hosting.Python.CreateEngine();
            var memoryStream = new MemoryStream();
            var writer = new EventRaisingStreamWriter(memoryStream);
            writer.StringWritten += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Value))
                {
                    logger.LogInformation($"Python - {e.Value}");
                }
            };
            engine.Runtime.IO.SetOutput(memoryStream, writer);
            engine.Runtime.IO.SetErrorOutput(memoryStream, writer);
            return engine;
        }
    }
}
