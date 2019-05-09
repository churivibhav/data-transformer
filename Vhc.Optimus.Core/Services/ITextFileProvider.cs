using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vhc.Optimus.Core.Services
{
    public interface ITextFileProvider
    {
        Task<IEnumerable<string>> ListFileKeysAsync(string path, string filePatternRegex = null);
        Task<string> GetContentAsync(string path, string key);
        string GetAbsolutePath(string unitPath, string jobKey);
    }
}
