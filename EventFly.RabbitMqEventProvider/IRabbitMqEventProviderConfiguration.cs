using System;

namespace EventFly.RabbitMqEventProvider
{
    public interface IRabbitMqEventProviderConfiguration
    {
        Uri Connection { get; set; }
        String ExchangeName { get; set; }
        String QueueName { get; set; }
    }

    internal sealed class RabbitMqEventProviderConfiguration : IRabbitMqEventProviderConfiguration
    {
        public Uri Connection { get; set; }

        public String ExchangeName { get; set; }

        public String QueueName { get; set; }
    }
}
