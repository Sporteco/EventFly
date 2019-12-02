using EventFly.Aggregates;

namespace EventFly.Events
{
    public interface IDomainEventBus
    {
        void Publish(IDomainEvent domainEvent);
    }
}
