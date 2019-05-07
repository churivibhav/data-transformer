using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vhc.DataTransformer.Core.Services;

namespace Vhc.DataTransformer.ReferenceImpl
{
    internal class PlainTextFileProvider : ITextFileProvider
    {
        private readonly ILogger<PlainTextFileProvider> logger;

        public PlainTextFileProvider(ILogger<PlainTextFileProvider> logger)
        {
            this.logger = logger;
        }

        public static string Separator = $"{Path.DirectorySeparatorChar}";

        public string GetAbsolutePath(string unitPath, string jobKey)
        {
            string currentFolder = $"./";
            if (unitPath.StartsWith(currentFolder))
            {
                return GetFolderFromJobKey(jobKey) + unitPath.Replace(currentFolder, string.Empty);
            }
            return unitPath;
        }

        private static string GetFolderFromJobKey(string key)
        {
            var parts = key.Split(Separator);
            return $"{string.Join(Separator, (from part in parts where part != parts.Last() select part).ToArray())}{Separator}";
        }

        public async Task<string> GetContentAsync(string path, string key)
            => await File.ReadAllTextAsync(key);



        public async Task<IEnumerable<string>> ListFileKeysAsync(string path, string filePatternRegex = null)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] files = d.GetFiles(filePatternRegex ?? "*", SearchOption.AllDirectories);
            return files.Select(f => f.FullName).ToList();
        }
    }
}