using System.Collections.Generic;

namespace Vhc.Optimus.DataObjects
{
    public class JobDataObject
    {
        public bool Parent { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public string ConnectionName { get; set; }
        public string Warehouse { get; set; }
        public IEnumerable<JobUnitDataObject> Units { get; set; }
        public IEnumerable<VariableDataObject> Variables { get; set; }
        public IDictionary<string, string> Properties { get; set; }
        public string Path { get; set; }
    }
}
