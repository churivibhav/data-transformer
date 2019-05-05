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
        private readonly string jobLocation;
        private readonly ILogger<PlainTextFileProvider> logger;

        public PlainTextFileProvider(string jobLocation, ILogger<PlainTextFileProvider> logger)
        {
            this.jobLocation = jobLocation;
            this.logger = logger;
        }

        public async Task<string> GetContentAsync(string path, string key)
        {
            string text = null;
            try
            {
                text = await File.ReadAllTextAsync(jobLocation + "/" + path + "/" + key);
            } catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
            return text;
        }

        public async Task<IEnumerable<string>> ListFileKeysAsync(string path, string filePatternRegex = null)
        {
            DirectoryInfo d = new DirectoryInfo(jobLocation + "/" + path);//Assuming Test is your Folder
            FileInfo[] files = d.GetFiles(filePatternRegex ?? "*");
            return files.Select(f => f.Name).ToList();
        }
    }
}