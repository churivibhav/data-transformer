using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vhc.Optimus.Core.Services
{
    public interface INotificationService
    {
        Task NotifyFailureAsync(string message);
    }
}
