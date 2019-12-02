using EventFly.Aggregates;
using EventFly.Definitions;
using EventFly.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventFly.ExternalEventProvider
{
    public abstract class AbstractExternalEventProvider
    {
        private readonly IReadOnlyCollection<Type> _eventTypes;
        private readonly IDomainEventBus _eventBus;

        protected AbstractExternalEventProvider(IApplicationDefinition applicationDefinition, IDomainEventBus eventBus)
        {
            _eventTypes = applicationDefinition.ExternalEvents.GetAllDefinitions().Select(d => d.Type).ToList();
            _eventBus = eventBus;
        }

        protected void Provide(IDomainEvent domainEvent)
        {
            if (_eventTypes.Contains(domainEvent.EventType) == false)
                return;

            _eventBus.Publish(domainEvent);
        }
    }
}
