using RabbitMQ.Client.Events;
using System.Linq;
using System.Text;

namespace EventFly.RabbitMqEventProvider
{
    internal sealed class RabbitMqMessageFactory : IRabbitMqMessageFactory
    {
        public RabbitMqMessage Create(BasicDeliverEventArgs basicDeliverEventArgs)
        {
            var messageId = basicDeliverEventArgs.BasicProperties.MessageId;
            var message = Encoding.UTF8.GetString(basicDeliverEventArgs.Body);
            var headers = basicDeliverEventArgs.BasicProperties.Headers.ToDictionary(kv => kv.Key, kv => kv.Value);

            return new RabbitMqMessage(
                messageId: messageId,
                message: message,
                headers: headers
            );
        }
    }
}
