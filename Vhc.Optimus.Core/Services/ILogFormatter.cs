using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Vhc.Optimus.Core.Services
{
    public interface ILogFormatter
    {
        ILogFormatter ForJob(string jobName, string db);
        ILogFormatter ForUnit(string unitName, string unitType);
        string TextForJob(string duration, string timeRemaining, string message);
        string TextForUnit(string duration, string timeRemaining, string returnValue, string message);
    }

    public class LogFormatter : ILogFormatter
    {
        private string jobName;
        private string db;
        private string unitName;
        private string unitType;

        public ILogFormatter ForJob(string jobName, string db)
        {
            this.jobName = jobName;
            this.db = db;
            return this;
        }

        public ILogFormatter ForUnit(string unitName, string unitType)
        {
            this.unitName = unitName;
            this.unitType = unitType;
            return this;
        }

        public string TextForJob(string duration, string timeRemaining, string message)
            => $"JOB {jobName?.PadRight(25)} | DB {db?.PadRight(23)} | ET {duration.PadLeft(12)} | RT {timeRemaining.PadLeft(12)} | {string.Empty.PadLeft(19)} | {message}";

        public string TextForUnit(string duration, string timeRemaining, string returnValue, string message)
            => $"JOB {jobName?.PadRight(25)} | UNIT {unitName?.PadRight(30)} | {unitType?.PadLeft(10)} | ET {duration.PadLeft(12)} | RT {timeRemaining.PadLeft(12)} | RETURNED {returnValue?.PadLeft(10)} | {message}";
    }
}
