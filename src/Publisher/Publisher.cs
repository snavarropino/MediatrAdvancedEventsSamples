using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher
{
    public class Publisher
    {
        private readonly ServiceFactory _serviceFactory;

        public Publisher(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            PublishStrategies[PublishStrategy.ParallelWait] = new CustomMediator(_serviceFactory, ParallelWait);
            PublishStrategies[PublishStrategy.SyncStopOnException] = new CustomMediator(_serviceFactory, SyncStopOnException);
            PublishStrategies[PublishStrategy.ParallelNoWait] = new CustomMediator(_serviceFactory, ParallelNoWait);
        }

        public IDictionary<PublishStrategy, IMediator> PublishStrategies = new Dictionary<PublishStrategy, IMediator>();

        public Task Publish<TNotification>(TNotification notification, PublishStrategy strategy) where TNotification : INotification
        {
            return Publish(notification, strategy, default(CancellationToken));
        }

        public Task Publish<TNotification>(TNotification notification, PublishStrategy strategy, CancellationToken cancellationToken) where TNotification : INotification
        {
            if (!PublishStrategies.TryGetValue(strategy, out var mediator))
            {
                throw new ArgumentException($"Unknown strategy: {strategy}");
            }

            return mediator.Publish(notification, cancellationToken);
        }

        private Task ParallelWait(IEnumerable<Func<Task>> handlers)
        {
            var tasks= new List<Task>();
            foreach (var handler in handlers)
            {
                tasks.Add(Task.Run(async () => await handler()));
            }

            Task.WaitAll(tasks.ToArray());

            return Task.CompletedTask;
        }

        private Task ParallelNoWait(IEnumerable<Func<Task>> handlers)
        {
            foreach (var handler in handlers)
            {
                Task.Run(async () => await handler());
            }

            return Task.CompletedTask;
        }

        private async Task SyncStopOnException(IEnumerable<Func<Task>> handlers)
        {
            foreach (var handler in handlers)
            {
                await handler();
            }
        }        
    }
}