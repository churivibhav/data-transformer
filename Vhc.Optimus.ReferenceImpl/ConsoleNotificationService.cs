using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vhc.Optimus.Core.Services;

namespace Vhc.Optimus.ReferenceImpl
{
    class ConsoleNotificationService : INotificationService
    {
        public Task NotifyFailureAsync(string message)
        {
            Console.WriteLine($"{this.GetType().Name} - {message}");
            return Task.CompletedTask;
        }
    }
}
