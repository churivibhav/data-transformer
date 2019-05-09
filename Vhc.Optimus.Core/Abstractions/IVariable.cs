namespace Vhc.Optimus.Core.Abstractions
{
    public interface IVariable
    {
        bool Active { get; set; }
        string Name { get; set; }
        string Value { get; set; }
    }
}