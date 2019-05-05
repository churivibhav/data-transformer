using System.Collections.Generic;
using Vhc.DataTransformer.Core.Models;

namespace Vhc.DataTransformer.Core.Models
{
    internal class Transformations
    {
        public Transformations()
        {
            Jobs = new List<Job>();
        }

        public ICollection<Job> Jobs { get; set; }
    }
}