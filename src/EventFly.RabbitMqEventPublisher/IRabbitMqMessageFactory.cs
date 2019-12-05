using EventFly.Aggregates;

namespace EventFly.RabbitMqEventPublisher
{
    internal interface IRabbitMqMessageFactory
    {
        RabbitMqMessage Create(IDomainEvent domainEvent);
    }
}
