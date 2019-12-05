using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Threading.Tasks;

namespace EventFly.RabbitMqEventPublisher
{
    internal static class ConnectionFactoryExtensions
    {
        public static async Task<IConnection> TryCreateConnection(this IConnectionFactory connectionFactory, Int32 attemptCount)
        {
            var attempt = 0;
            while (attempt < attemptCount)
            {
                try
                {
                    return connectionFactory.CreateConnection();
                }
                catch (BrokerUnreachableException)
                {
                    attempt++;
                }

                await Task.Delay((Int32)Math.Truncate(attempt * 1.5) * 1000);
            }

            throw new Exception("Connection is not reachable");
        }
    }
}
