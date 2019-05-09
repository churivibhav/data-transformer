using System.Data;
using Microsoft.Scripting.Hosting;

namespace Vhc.Optimus.Core.Services
{
    public interface IConnectionProvider
    {
        IDbConnection NewConnection { get; }

    }
}