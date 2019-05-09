using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vhc.Optimus.Core.Abstractions;
using Vhc.Optimus.Core.Utils;

namespace Vhc.Optimus.Core.Models
{
    public class PythonJobUnit : IJobUnit
    {
        public string UnitType => "Python";

        public string Name { get; set; }
        public string Content { get; set; }
        public IDictionary<string, string> Properties { get; set; }

        public IResult Execute(IResources resources)
        {
            var engine = resources.ScriptEngine;
            var scope = engine.CreateScope();
            foreach (var variable in resources.Variables)
            {
                scope.SetVariable(variable.Key, variable.Value);
            }

            AddScopeMethods(resources, scope);

            resources.Logger.LogTrace($"Python Unit content : {Content}");

            var source = engine.CreateScriptSourceFromString(Content);
            try
            {
                var result = source.Execute(scope);
            }
            catch (SyntaxErrorException ex)
            {
                return new Result
                {
                    Success = false,
                    Message = $"FAILED UNIT - {Name} \n SYNTAX ERROR - {ex.Message} \n PYTHON - {Content}"
                };
            }
            int recordsAffected = 0;
            foreach (var variable in new Dictionary<string, string>(resources.Variables))
            {
                string newValue = scope.GetVariable(variable.Key).ToString();
                if (newValue != variable.Value)
                {
                    resources.Variables[variable.Key] = newValue;
                    recordsAffected++;
                }
            }
            return new Result
            {
                Success = true,
                Message = $"COMPLETED UNIT - {Name}",
                RecordsAffected = recordsAffected
            };
        }

        private static void AddScopeMethods(IResources resources, Microsoft.Scripting.Hosting.ScriptScope scope)
        {

            Action<string> log = (logText) => resources.Logger.LogInformation($"[Python] - {logText}");
            Func<string, int> sql = (sqlText) => resources.DbTransaction.Connection.Execute(sqlText, transaction: resources.DbTransaction, commandTimeout: resources.Context.MaximumTimeout);
            Action commit = () => resources.DbTransaction.Commit();
            Action rollback = () => resources.DbTransaction.Rollback();
            Func<string, IEnumerable<IronPython.Runtime.PythonDictionary>> query = (sqlText) => resources.DbTransaction.Connection.Query(
                sqlText, transaction: resources.DbTransaction, commandTimeout: resources.Context.MaximumTimeout
                ).ToList().Select(row => (row as IDictionary<string, object>).ToPythonDictionary()).ToList();

            scope.SetVariable("log", log);
            scope.SetVariable("sql", sql);
            scope.SetVariable("commit", commit);
            scope.SetVariable("rollback", rollback);
            scope.SetVariable("query", query);
        }
    }
}
