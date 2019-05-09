using Vhc.Optimus.Core.Abstractions;

namespace Vhc.Optimus.Core.Models
{
    public class Variable : IVariable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Active { get; set; }
    }
}