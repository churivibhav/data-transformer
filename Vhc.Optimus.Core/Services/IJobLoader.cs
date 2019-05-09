using System.Collections.Generic;
using System.Threading.Tasks;
using Vhc.Optimus.Core.Abstractions;

namespace Vhc.Optimus.Core.Services
{
    public interface IJobLoader
    {
        Task<ICollection<IJob>> LoadAllParentJobsAsync(ICriteria criteria);
        Task<IJob> LoadJobAsync(string path);
        IJob LoadJobByContent(string content);
        Task<string> GetContentByPathAsync(string path);

        IJobUnit CreateUnitByType(string unitType);
    }
}