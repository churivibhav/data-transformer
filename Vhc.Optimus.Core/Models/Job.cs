using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Vhc.Optimus.Core.Abstractions;
using Vhc.Optimus.Core.Services;
using Vhc.Optimus.Core.Utils;

namespace Vhc.Optimus.Core.Models
{
    public class Job : IJob
    {
        public string UnitType => "Job";

        public bool Parent { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public string ConnectionName { get; set; }
        public string Warehouse { get; set; }
        public IEnumerable<IJobUnit> Units { get; set; }
        public IEnumerable<IVariable> Variables { get; set; }
        public IDictionary<string, string> Properties { get; set; }
        public string Content { get; set; }

        public override string ToString() => $"JOB {Name}";

        public IResult Execute(IResources resources)
        {
            if (resources is null) throw new ArgumentNullException(nameof(resources));

            // Make a copy of variables
            resources.Variables = new ConcurrentDictionary<string, string>(Variables.Where(v => v.Active)
                                                    .Select(v => new KeyValuePair<string, string>(v.Name, v.Value)));

            foreach (var unit in Units)
            {
                // Check time remaining
                if (resources.Context.IsNearingCompletion)
                {
                    throw new ContextTimeConstraintException($"Aborting Job - {Name} - Low Context Time Remaining : {resources.Context.RemainingTime}");
                }

                var formatter = resources.LogFormatter.ForUnit(unit.Name, unit.UnitType);
                var unitTimer = new PerformanceTimer().Start();
                IJobUnit currentUnit =
                    unit is Job ? resources.JobLoader.LoadJobByContent(unit.Content) : unit;
                var result = currentUnit.Execute(resources);
                var elapsedTime = unitTimer.StopAndGetElapsedTime();
                resources.Logger.LogInformation(formatter.TextForUnit(elapsedTime.ToFormattedString(), resources.Context.RemainingTime.ToFormattedString(), result.RecordsAffected.ToString(), result.Message));
                if (!result.Success && resources.AbortOnFailure)
                {
                    return new Result
                    {
                        Message = $"ABORTED JOB - {Name} \n {result.Message}",
                        Success = false
                    };
                }
            }
            return new Result
            {
                Message = $"COMPLETED JOB - {Name}",
                Success = true
            };

        }
    }
}
