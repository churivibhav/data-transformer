using System.Data;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Scripting.Hosting;
using Vhc.DataTransformer.Core.Services;
using Vhc.DataTransformer.Services;

namespace Vhc.DataTransformer.ReferenceImpl
{
    internal class SqliteConnectionProvider : IConnectionProvider
    {
        private readonly ILogger<SqliteConnectionProvider> logger;

        public SqliteConnectionProvider(ILogger<SqliteConnectionProvider> logger)
        {
            this.logger = logger;
            ScriptEngine = CreateScriptEngine();
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

        public IDbConnection NewConnection => new System.Data.SQLite.SQLiteConnection();

        public ScriptEngine ScriptEngine { get; }
    }
}