using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vhc.DataTransformer.Core.Services
{
    public interface INotificationService
    {
        Task NotifyFailureAsync(string message);
    }
}
