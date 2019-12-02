using EventFly.Aggregates;
using EventFly.Extensions;
using EventFly.ExternalEventPublisher;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventFly.RabbitMqEventPublisher
{
    public sealed class RabbitMqEventPublisher : IExternalEventPublisher, IDisposable
    {
        private readonly IRabbitMqEventPublisherConfiguration _configuration;
        private readonly IRabbitMqMessageFactory _rabbitMqMessageFactory;

        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private Boolean _initialized;

        private readonly SemaphoreSlim _connectionSemaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _initSemaphoreSlim = new SemaphoreSlim(1, 1);

        public RabbitMqEventPublisher(IRabbitMqEventPublisherConfiguration configuration)
        {
            _configuration = configuration;
            _rabbitMqMessageFactory = new RabbitMqMessageFactory();

            _connectionFactory = new ConnectionFactory { Uri = _configuration.Connection };
        }

        public async Task Publish(IDomainEvent domainEvent)
        {
            var connection = await GetConnection();

            using (var channel = connection.CreateModel())
            {

                await Initialize(channel);

                var message = _rabbitMqMessageFactory.Create(domainEvent);
                var properties = channel.CreateBasicProperties();
                properties.Headers = message.Headers.ToDictionary(kv => kv.Key, kv => kv.Value);
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.Now.ToUnixTime());
                properties.ContentEncoding = "utf-8";
                properties.ContentType = "application/json";
                properties.MessageId = message.MessageId;

                channel.ConfirmSelect();
                channel.BasicPublish(_configuration.ExchangeName, "", false, properties, Encoding.UTF8.GetBytes(message.Message));
                channel.WaitForConfirms();
            }
        }

        private async Task<IConnection> GetConnection()
        {
            if (_connection != null && _connection.IsOpen)
                return _connection;

            await _connectionSemaphoreSlim.WaitAsync();

            try
            {
                if (_connection != null && _connection.IsOpen)
                    return _connection;

                _connection = await _connectionFactory.TryCreateConnection(5);

                return _connection;
            }
            finally
            {
                _connectionSemaphoreSlim.Release();
            }
        }

        private async Task Initialize(IModel channel)
        {
            if (_initialized)
                return;

            await _initSemaphoreSlim.WaitAsync();

            try
            {
                if (_initialized)
                    return;

                channel.ExchangeDeclare(_configuration.ExchangeName, ExchangeType.Fanout);

                _initialized = true;
            }
            finally
            {
                _initSemaphoreSlim.Release();
            }
        }

        Boolean _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _connection.Close(3000);
                _connection?.Dispose();
            }

            _disposed = true;
        }
    }
}
