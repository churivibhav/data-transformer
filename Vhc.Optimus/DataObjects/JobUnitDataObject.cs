using System.Collections.Generic;

namespace Vhc.Optimus.DataObjects
{
    public class JobUnitDataObject
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public IDictionary<string, string> Properties { get; set; }
        public string Type { get; set; }
    }
}