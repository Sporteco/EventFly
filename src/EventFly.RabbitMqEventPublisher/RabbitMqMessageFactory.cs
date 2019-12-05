using EventFly.Aggregates;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace EventFly.RabbitMqEventPublisher
{
    internal sealed class RabbitMqMessageFactory : IRabbitMqMessageFactory
    {
        public RabbitMqMessage Create(IDomainEvent domainEvent)
        {
            var messageId = domainEvent.Metadata.EventId.Value;
            var message = JsonConvert.SerializeObject(domainEvent.GetAggregateEvent());
            var headers = domainEvent.Metadata.ToDictionary(kv => kv.Key, kv => (Object)kv.Value);

            return new RabbitMqMessage(
                messageId: messageId,
                message: message,
                headers: headers
            );
        }
    }
}
