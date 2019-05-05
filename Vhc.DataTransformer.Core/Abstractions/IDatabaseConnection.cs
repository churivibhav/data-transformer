using System;
using System.Collections.Generic;
using System.Text;

namespace Vhc.DataTransformer.Core.Abstractions
{
    public interface IDatabaseConnection
    {
        string Name { get; set; }
        string ConnectionString { get; set; }
    }
}
