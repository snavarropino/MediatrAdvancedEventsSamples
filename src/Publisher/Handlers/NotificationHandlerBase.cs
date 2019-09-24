using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Publisher.Handlers
{
    public abstract class NotificationHandlerBase<T> : INotificationHandler<T> where T : INotification
    {
        public async Task Handle(T notification, CancellationToken cancellationToken)
        {
            if (HasToSupressTransaction())
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await Execute(notification, cancellationToken);
                }               
            }
            else
            { 
                await Execute(notification, cancellationToken);
            }
        }

        protected abstract Task Execute (T notification, CancellationToken cancellationToken);

        private bool HasToSupressTransaction ()
        {
            return Attribute.IsDefined(this.GetType(), typeof(SupressTransactionAttribute));
        }
    }
}