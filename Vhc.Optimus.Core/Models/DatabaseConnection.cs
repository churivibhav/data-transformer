using System;
using System.Collections.Generic;
using System.Text;
using Vhc.Optimus.Core.Abstractions;

namespace Vhc.Optimus.Core.Models
{
    public class DatabaseConnection : IDatabaseConnection
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}
