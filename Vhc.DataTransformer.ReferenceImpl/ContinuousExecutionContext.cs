using System;
using Vhc.DataTransformer.Core.Abstractions;

namespace Vhc.DataTransformer.ReferenceImpl
{
    internal class ContinuousExecutionContext : IExecutionContext
    {
        private static TimeSpan threshold = new TimeSpan(hours: 0, minutes: 2, seconds: 0);
        public bool IsNearingCompletion => this.RemainingTime <= threshold;

        public TimeSpan RemainingTime => new TimeSpan(hours: 0, minutes: 10, seconds: 0);

        public int MaximumTimeout => IsNearingCompletion ? 0 : Convert.ToInt32((this.RemainingTime - threshold).TotalSeconds);
    }
}