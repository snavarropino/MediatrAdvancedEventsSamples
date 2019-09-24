using Publisher.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher.Handlers
{
    [SupressTransaction]
    public class AsyncPingedSupressTransactionHandler : NotificationHandlerBase<Pinged>
    {
        private readonly PingContext _context;

        public AsyncPingedSupressTransactionHandler(PingContext context)
        {
            _context = context;
        }

        protected override async Task Execute(Pinged notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[AsyncPingedTransactionHandler ({notification.Value})] {DateTime.Now:HH:mm:ss.fff} : Pinged Handler");

            _context.Pings.Add(new Ping() { Value = notification.Value });
            await _context.SaveChangesAsync();
            await Task.Delay(notification.DelayInMs);

            Console.WriteLine($"[AsyncPingedTransactionHandler ({notification.Value})] {DateTime.Now:HH:mm:ss.fff} : After Pinged Handler");
        }
    }
}