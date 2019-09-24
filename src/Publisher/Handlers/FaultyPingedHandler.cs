using System;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher.Handlers
{
    public class FaultyPingedHandler : NotificationHandlerBase<Pinged>
    {
        protected override async Task Execute(Pinged notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[FaultyPingedHandler ({notification.Value})] {DateTime.Now:HH:mm:ss.fff} : Pinged Handler");
            await Task.Delay(notification.DelayInMs);
            Console.WriteLine($"[FaultyPingedHandler ({notification.Value})] {DateTime.Now:HH:mm:ss.fff} : Throwing exception");
            throw new Exception($"Exception handling Pinged ({notification.Value})");
        }
    }
}