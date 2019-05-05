using System;

namespace Vhc.DataTransformer.Core.Models
{
    public class ContextTimeConstraintException : Exception
    {
        public ContextTimeConstraintException(string message) : base(message)
        {
        }
    }
}
