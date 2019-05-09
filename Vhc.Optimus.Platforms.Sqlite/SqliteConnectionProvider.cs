using System.Data;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Scripting.Hosting;
using Vhc.Optimus.Core.Services;
using Vhc.Optimus.Services;

namespace Vhc.Optimus.Platforms.Sqlite
{
    public class SqliteConnectionProvider : IConnectionProvider
    {
        public IDbConnection NewConnection => new System.Data.SQLite.SQLiteConnection();
    }
}