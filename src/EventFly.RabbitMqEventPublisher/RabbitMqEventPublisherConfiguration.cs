using System;

namespace EventFly.RabbitMqEventPublisher
{
    public sealed class RabbitMqEventPublisherConfiguration : IRabbitMqEventPublisherConfiguration
    {
        public static IRabbitMqEventPublisherConfiguration With(Uri connection, String exchangeName)
            => new RabbitMqEventPublisherConfiguration(connection, exchangeName);

        private RabbitMqEventPublisherConfiguration(Uri connection, String exchange)
        {
            Connection = connection;
            ExchangeName = exchange;
        }

        public Uri Connection { get; }
        public String ExchangeName { get; }
    }
}
