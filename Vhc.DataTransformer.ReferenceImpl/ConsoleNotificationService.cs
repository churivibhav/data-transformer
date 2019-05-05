using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vhc.DataTransformer.Core.Services;

namespace Vhc.DataTransformer.ReferenceImpl
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
