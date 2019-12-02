using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventFly.Aggregates;
using EventFly.Definitions;
using EventFly.ExternalEventPublisher;
using EventFly.Subscribers;

namespace EventFly.Infrastructure.Subscribers
{
    internal sealed class ExternalEventPublisher<TContextDefinition> : DomainEventSubscriber, ISubscribeToManyAsync
        where TContextDefinition : IContextDefinition
    {
        private readonly TContextDefinition _contextDefinition;
        private readonly IExternalEventPublisher _publisher;

        public ExternalEventPublisher(TContextDefinition contextDefinition, IExternalEventPublisher publisher)
        {
            _contextDefinition = contextDefinition;
            _publisher = publisher;
        }

        public async Task HandleAsync(IDomainEvent domainEvent)
        {
            await _publisher.Publish(domainEvent);
        }

        public IEnumerable<Type> GetEventTypes()
        {
            return _contextDefinition.PublicEvents.GetAllDefinitions().Select(d => d.Type);
        }
    }
}
