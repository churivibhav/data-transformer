using System;
using Vhc.Optimus.Core.Abstractions;

namespace Vhc.Optimus.Platforms.Sqlite
{
    public class ContinuousExecutionContext : IExecutionContext
    {
        private static TimeSpan threshold = new TimeSpan(hours: 0, minutes: 2, seconds: 0);
        public bool IsNearingCompletion => RemainingTime <= threshold;

        public TimeSpan RemainingTime => new TimeSpan(hours: 0, minutes: 10, seconds: 0);

        public int MaximumTimeout => IsNearingCompletion ? 0 : Convert.ToInt32((RemainingTime - threshold).TotalSeconds);
    }
}