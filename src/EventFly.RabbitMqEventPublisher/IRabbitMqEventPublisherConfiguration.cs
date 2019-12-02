using System;

namespace EventFly.RabbitMqEventPublisher
{
    public interface IRabbitMqEventPublisherConfiguration
    {
        Uri Connection { get; }
        String ExchangeName { get; }
    }
}
