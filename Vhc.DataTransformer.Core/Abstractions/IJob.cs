using System.Collections.Generic;

namespace Vhc.DataTransformer.Core.Abstractions
{
    public interface IJob : IJobUnit
    {
        bool Parent { get; set; }
        bool Active { get; set; }
        string Description { get; set; }
        /// <summary>
        /// The higher the priority, the earlier is this job picked up for running
        /// </summary>
        int Priority { get; set; }
        IEnumerable<IJobUnit> Units { get; set; }
        IEnumerable<IVariable> Variables { get; set; }

        /// <summary>
        /// The name of the database connection to run this job on
        /// </summary>
        string ConnectionName { get; set; }
    }
}