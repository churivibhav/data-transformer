using System.Collections.Generic;
using Vhc.Optimus.Core.Models;

namespace Vhc.Optimus.Core.Models
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