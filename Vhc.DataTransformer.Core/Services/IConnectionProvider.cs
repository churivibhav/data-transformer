using System.Data;
using Microsoft.Scripting.Hosting;

namespace Vhc.DataTransformer.Core.Services
{
    public interface IConnectionProvider
    {
        IDbConnection NewConnection { get; }
        ScriptEngine ScriptEngine { get; }
    }
}