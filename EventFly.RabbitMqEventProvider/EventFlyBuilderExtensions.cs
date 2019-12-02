using EventFly.Definitions;
using EventFly.DependencyInjection;
using EventFly.Events;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventFly.RabbitMqEventProvider
{
    public static class EventFlyBuilderExtensions
    {
        public static EventFlyBuilder AddRabbitMqDomainEventProvider(this EventFlyBuilder eventFlyBuilder, Action<IRabbitMqEventProviderConfiguration> configure)
        {
            if (configure is null)
                throw new ArgumentNullException(nameof(configure));

            var cfg = new RabbitMqEventProviderConfiguration();
            configure(cfg);

            eventFlyBuilder.Services.AddHostedService(sp =>
            {
                var appDefinition = sp.GetService<IApplicationDefinition>();
                var domainEventFactory = sp.GetService<IDomainEventFactory>();
                var domainEventBus = sp.GetService<IDomainEventBus>();

                return new RabbitMqEventProvider(cfg, appDefinition, domainEventFactory, domainEventBus);
            });

            return eventFlyBuilder;
        }
    }
}
