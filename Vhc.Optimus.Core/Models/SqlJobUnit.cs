using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Vhc.Optimus.Core.Abstractions;
using Vhc.Optimus.Core.Models;

namespace Vhc.Optimus.Core.Models
{
    public class SqlJobUnit : IJobUnit
    {
        public string UnitType => "SQL";

        public string Name { get; set; }
        public string Content { get; set; }
        public IDictionary<string, string> Properties { get; set; }

        public IResult Execute(IResources resources)
        {
            int count = 0;
            string sql = Content;
            foreach (var variable in resources.Variables)
            {
                sql = sql.Replace(string.Join(string.Empty, "${", variable.Key, "}"), variable.Value);
            }
            resources.Logger.LogTrace($"Sql Job Unit Content : {sql}");
            try
            {
                count = resources.DbTransaction.Connection.Execute(sql, transaction: resources.DbTransaction, commandTimeout: resources.Context.MaximumTimeout);
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Message = $"FAILED UNIT - {Name} \n EXCEPTION - {ex.GetType().Name} - {ex.Message} \n SQL - {sql}",
                    Success = false,
                    RecordsAffected = count
                };
            }

            return new Result
            {
                Message = $"COMPLETED UNIT - {Name}",
                Success = true,
                RecordsAffected = count
            };
        }
    }
}
