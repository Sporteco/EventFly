using EventFly.Aggregates;
using EventFly.Definitions;
using EventFly.Events;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventFly.RabbitMqEventProvider
{
    internal sealed class RabbitMqEventProvider : IHostedService
    {
        private readonly IRabbitMqEventProviderConfiguration _configuration;
        private readonly IRabbitMqMessageFactory _rabbitMqMessageFactory;

        private readonly IApplicationDefinition _applicationDefinition;
        private readonly IDomainEventFactory _domainEventFactory;
        private readonly IDomainEventBus _domainEventBus;

        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer _consumer;

        private Boolean _isRunnig;

        public RabbitMqEventProvider(IRabbitMqEventProviderConfiguration configuration, IApplicationDefinition applicationDefinition, IDomainEventFactory domainEventFactory, IDomainEventBus domainEventBus)
        {
            _configuration = configuration;
            _rabbitMqMessageFactory = new RabbitMqMessageFactory();

            _applicationDefinition = applicationDefinition;
            _domainEventFactory = domainEventFactory;
            _domainEventBus = domainEventBus;

            _connectionFactory = new ConnectionFactory { Uri = _configuration.Connection };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InternalStart();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            InternalStop();
            return Task.CompletedTask;
        }

        private async Task InternalStart()
        {
            _connection = await _connectionFactory.TryCreateConnection(20);
            _connection.ConnectionShutdown += OnConnectionShutdown;
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_configuration.ExchangeName, ExchangeType.Fanout);
            _channel.QueueDeclare(_configuration.QueueName, true, false, false);
            _channel.QueueBind(_configuration.QueueName, _configuration.ExchangeName, "");

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += OnReceived;

            _channel.BasicConsume(_configuration.QueueName, false, _consumer);

            _isRunnig = true;
        }

        private void InternalStop()
        {
            _consumer.Received -= OnReceived;

            if (_connection.IsOpen)
                _channel.BasicCancel(_consumer.ConsumerTags.FirstOrDefault());

            _channel.Dispose();

            _connection.ConnectionShutdown -= OnConnectionShutdown;

            if (_connection.IsOpen)
                _connection.Close();
        }

        private void OnConnectionShutdown(Object sender, ShutdownEventArgs e)
        {
            do
            {
                try
                {
                    if (_isRunnig == false)
                        return;

                    InternalStop();
                    InternalStart().GetAwaiter().GetResult();
                }
                catch
                {
                    //ignore
                }
            }
            while (true);
        }

        private void OnReceived(Object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var message = _rabbitMqMessageFactory.Create(e);
                var eventMetadata = new EventMetadata(message.Headers.ToDictionary(kv => kv.Key, kv => Encoding.UTF8.GetString((Byte[])kv.Value)));

                var eventDefinition = _applicationDefinition.Events.GetDefinition(eventMetadata.EventName, eventMetadata.EventVersion);

                if (eventDefinition == null)
                {
                    _channel.BasicAck(e.DeliveryTag, false);
                    return;
                }

                var aggregateEvent = (IAggregateEvent)JsonConvert.DeserializeObject(message.Message, eventDefinition.Type);

                var domainEvent = _domainEventFactory.Create(aggregateEvent, eventMetadata);

                _domainEventBus.Publish(domainEvent);
            }
            catch
            {
                if (_connection.IsOpen)
                    _channel.BasicNack(e.DeliveryTag, false, false);
            }

            if (_connection.IsOpen)
                _channel.BasicAck(e.DeliveryTag, false);
        }
    }
}
