using MediatR;
using Publisher.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher.Handlers
{
    public class AsyncPingedHandler : NotificationHandlerBase<Pinged>
    {
        private readonly PingContext _context;

        public AsyncPingedHandler(PingContext context)
        {
            _context = context;
        }

        protected override async Task Execute(Pinged notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[AsyncPingedHandler ({notification.Value})] {DateTime.Now:HH:mm:ss.fff} : Pinged Handler");
            
            _context.Pings.Add(new Ping() { Value= notification.Value});
            await _context.SaveChangesAsync();
            await Task.Delay(notification.DelayInMs);

            Console.WriteLine($"[AsyncPingedHandler ({notification.Value})] {DateTime.Now:HH:mm:ss.fff} : After Pinged Handler");
        }
    }
}