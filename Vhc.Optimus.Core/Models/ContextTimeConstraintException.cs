using System;

namespace Vhc.Optimus.Core.Models
{
    public class ContextTimeConstraintException : Exception
    {
        public ContextTimeConstraintException(string message) : base(message)
        {
        }
    }
}
