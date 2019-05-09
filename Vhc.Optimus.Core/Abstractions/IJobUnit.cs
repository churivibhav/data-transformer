using System.Collections.Generic;

namespace Vhc.Optimus.Core.Abstractions
{
    public interface IJobUnit
    {
        string Name { get; set; }
        string Content { get; set; }
        IDictionary<string, string> Properties { get; set; }
        string UnitType { get; }

        IResult Execute(IResources resources);
    }
}