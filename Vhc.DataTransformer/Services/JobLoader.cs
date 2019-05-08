using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vhc.DataTransformer.Core.Abstractions;
using Vhc.DataTransformer.Core.Models;
using Vhc.DataTransformer.Core.Services;
using Vhc.DataTransformer.DataObjects;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Vhc.DataTransformer.Services
{
    public class JobLoader : IJobLoader
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<IJobLoader> logger;
        private readonly Func<string, IJobUnit> unitFactory;
        private readonly ITextFileProvider textFileProvider;

        private bool UseCloudFilesystem;
        private string bucketName;
        private string filePattern;
        private IDeserializer deserializer;

        public JobLoader(
            IConfiguration configuration,
            ITextFileProvider textFileProvider,
            ILogger<IJobLoader> logger,
            Func<string, IJobUnit> unitFactory)
        {
            this.configuration = configuration;
            this.textFileProvider = textFileProvider;
            this.logger = logger;
            this.unitFactory = unitFactory;
            Initialize();
        }

        public IJobUnit CreateUnitByType(string unitType) => unitFactory(unitType);


        private void Initialize()
        {
            UseCloudFilesystem = Convert.ToBoolean(configuration["Transformations:UseCloudFilesystem"]);
            bucketName = configuration["Transformations:BucketName"];
            filePattern = configuration["Transformations:FilePattern"];
            deserializer = new DeserializerBuilder()
                .WithNamingConvention(new PascalCaseNamingConvention())
                .Build();
        }

        public async Task<string> GetContentByPathAsync(string path)
            => UseCloudFilesystem
                ? await textFileProvider.GetContentAsync(bucketName, path)
                : await File.ReadAllTextAsync(path);

        public async Task<ICollection<IJob>> LoadAllParentJobsAsync(ICriteria criteria)
        {
            if (criteria is null) throw new ArgumentNullException(nameof(criteria));

            ICollection<JobDataObject> jobDataObjects = await GetAllJobsAsync(criteria.RunFolder);
            return ToJobCollection( jobDataObjects
                .Where(job => criteria.RunAll || criteria.RunJobs.Contains(job.Name)) // Take jobs specified by criteria, otherwise take all
                .Where(job => job.Active) // Take only active jobs
                .Select(job => SetJobVariables(job, criteria))
                , this);
        }

        private JobDataObject SetJobVariables(JobDataObject job, ICriteria criteria)
        {
            try
            {
                var adhocVariables = criteria.JobVariables[job.Name];

                if (adhocVariables != null && adhocVariables.Count > 0)
                {
                    var jobVariables = job.Variables;
                    job.Variables = jobVariables.Select(v =>
                    {
                        (bool useAdhocValue, string adhocValue) = GetAdhocValue(adhocVariables, v.Name);
                        return new VariableDataObject
                        {
                            Active = v.Active,
                            Name = v.Name,
                            Value = useAdhocValue ? adhocValue : v.Value
                        };

                        // inner function returns tuple with adhoc value if it is present
                        (bool, string) GetAdhocValue(IDictionary<string, string> adhocVars, string name)
                        {
                            try
                            {
                                return (true, adhocVars[name]);
                            }
                            catch (KeyNotFoundException e)
                            {
                                logger.LogWarning($"Ignored - Key Not found - {e.Message}");
                                // Do not use adhoc value if not found
                                return (false, null);
                            }
                            catch (Exception e)
                            {
                                logger.LogError("Error in getting job variable from criteria : " + e.Message);
                                return (false, null);
                            }
                        }
                    });
                }
            }
            catch (Exception)
            {
                logger.LogDebug("No adhoc variables found for job : " + job.Name);
            }
            return job;
        }



        private async Task<ICollection<JobDataObject>> GetAllJobsAsync(string folder = default)
        {
            var jobDataObjects = new List<JobDataObject>();
            var keys = await textFileProvider.ListFileKeysAsync(bucketName, filePattern);

            foreach (var key in keys)
            {
                if (folder == default || key.StartsWith(folder))
                {
                    JobDataObject job = await GetJobAsync(key);
                    job.Path = key;
                    jobDataObjects.Add(job);
                }
            }

            return jobDataObjects;
        }

        private async Task<JobDataObject> GetJobAsync(string key)
        {
            string content = await GetContentByPathAsync(key);
            var job = deserializer.Deserialize<JobDataObject>(content);
            return job;
        }

        public async Task<IJob> LoadJobAsync(string path)
        {
            JobDataObject jobDataObject = await GetJobAsync(path);
            jobDataObject.Path = path;
            return ToJob(jobDataObject, this);
        }

        public IJob LoadJobByContent(string content)
        {
            return ToJob(deserializer.Deserialize<JobDataObject>(content), this);
        }
    
        public ICollection<IJob> ToJobCollection(IEnumerable<JobDataObject> jobDataObjects, IJobLoader loader)
            => jobDataObjects
                .Where(j => j.Active && j.Parent)
                .Select(j => ToJob(j, loader))
                .ToList();

        private IJob ToJob(JobDataObject j, IJobLoader loader) => new Job
        {
            Name = j.Name,
            Active = j.Active,
            Description = j.Description,
            Parent = j.Parent,
            Priority = j.Priority,
            ConnectionName = j.ConnectionName,
            Warehouse = j.Warehouse,
            Units = ToUnitCollection(j.Units, loader, j.Path),
            Variables = ToVariableCollection(j.Variables),
            Properties = j.Properties
        } as IJob;

        private IEnumerable<IVariable> ToVariableCollection(IEnumerable<VariableDataObject> variableDataObjects)
            => variableDataObjects is null
                ? new List<Variable>()
                : variableDataObjects.Where(v => v.Active)
                    .Select(v => new Variable
                    {
                        Name = v.Name,
                        Value = v.Value,
                        Active = v.Active
                    })
                    .ToList();

        private IEnumerable<IJobUnit> ToUnitCollection(IEnumerable<JobUnitDataObject> jobUnitDataObjects, IJobLoader loader, string jobKey)
            => jobUnitDataObjects is null
                ? new List<IJobUnit>()
                : jobUnitDataObjects.Select(u =>
                {
                    IJobUnit unit = loader.CreateUnitByType(u.Type);
                    unit.Name = u.Name;
                    unit.Properties = u.Properties;
                    unit.Content = loader.GetContentByPathAsync(textFileProvider.GetAbsolutePath(u.Path, jobKey)).GetAwaiter().GetResult();
                    return unit;
                })
                .ToList();

        
    }

}

