using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Publisher
{
    public class CustomMediator : Mediator
    {
        private Func<IEnumerable<Func<Task>>, Task> _publish;

        public CustomMediator(ServiceFactory serviceFactory, Func<IEnumerable<Func<Task>>, Task> publish) : base(serviceFactory)
        {
            _publish = publish;
        }

        protected override Task PublishCore(IEnumerable<Func<Task>> allHandlers)
        {
            return _publish (allHandlers) ;
        }
    }
}