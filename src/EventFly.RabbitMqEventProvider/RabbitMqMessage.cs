using System;
using System.Collections.Generic;

namespace EventFly.RabbitMqEventProvider
{
    internal sealed class RabbitMqMessage
    {
        public RabbitMqMessage(String messageId, String message, IReadOnlyDictionary<String, Object> headers)
        {
            MessageId = messageId;
            Message = message;
            Headers = headers;
        }

        public String MessageId { get; }
        public String Message { get; }
        public IReadOnlyDictionary<String, Object> Headers { get; }
    }
}
