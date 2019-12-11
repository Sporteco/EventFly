using Akka.Actor;
using EventFly.Aggregates;
using EventFly.Events;

namespace EventFly.Infrastructure.Events
{
    public sealed class EventStreamDomainEventBus : IDomainEventBus
    {
        private readonly ActorSystem _actorSystem;

        public EventStreamDomainEventBus(ActorSystem actorSystem)
        {
            _actorSystem = actorSystem;
        }

        public void Publish(IDomainEvent domainEvent)
        {
            _actorSystem.EventStream.Publish(domainEvent);
        }
    }
}
