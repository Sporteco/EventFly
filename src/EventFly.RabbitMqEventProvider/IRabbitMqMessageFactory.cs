using RabbitMQ.Client.Events;

namespace EventFly.RabbitMqEventProvider
{
    internal interface IRabbitMqMessageFactory
    {
        RabbitMqMessage Create(BasicDeliverEventArgs basicDeliverEventArgs);
    }
}
