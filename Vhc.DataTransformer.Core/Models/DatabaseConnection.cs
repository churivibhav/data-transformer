using System;
using System.Collections.Generic;
using System.Text;
using Vhc.DataTransformer.Core.Abstractions;

namespace Vhc.DataTransformer.Core.Models
{
    public class DatabaseConnection : IDatabaseConnection
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}
