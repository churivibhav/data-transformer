using System;
using System.Collections.Generic;
using System.Text;

namespace Vhc.DataTransformer.Core.Abstractions
{
    public interface IExecutionContext
    {
        // Is the context going to end in near future
        bool IsNearingCompletion { get; }

        // Time remaining to end the context 
        TimeSpan RemainingTime { get; }

        // Time after which DbConnection should stop executing a running query
        int MaximumTimeout { get; }

    }
}
