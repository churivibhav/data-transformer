using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Vhc.Optimus.Core.Services
{
    public interface IPerformanceTimer
    {
        IPerformanceTimer Start();
        TimeSpan StopAndGetElapsedTime();
    }

    public class PerformanceTimer : IPerformanceTimer
    {
        private Stopwatch stopwatch;

        public IPerformanceTimer Start()
        {
            stopwatch = Stopwatch.StartNew();
            return this;
        }

        public TimeSpan StopAndGetElapsedTime()
        {
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }

}
