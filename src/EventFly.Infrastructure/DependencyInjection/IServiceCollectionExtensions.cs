using System;
using Akka.Actor;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.Core;
using EventFly.Events;
using EventFly.Infrastructure.Commands;
using EventFly.Infrastructure.Definitions;
using EventFly.Infrastructure.Events;
using EventFly.Infrastructure.Jobs;
using EventFly.Infrastructure.Permissions;
using EventFly.Infrastructure.Queries;
using EventFly.Infrastructure.Schedulers;
using EventFly.Permissions;
using EventFly.Queries;
using EventFly.Schedulers;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.Infrastructure.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceProvider UseEventFly(this IServiceProvider serviceProvider)
        {
            var actorSystem = serviceProvider.GetService<ActorSystem>();
            actorSystem.RegisterDependencyResolver(serviceProvider);
            if (!(serviceProvider.GetService<IDefinitionToManagerRegistry>() is DefinitionToManagerRegistry))
            {
                throw new InvalidOperationException("");
            }
            return serviceProvider;
        }

        public static EventFlyBuilder AddEventFly(this IServiceCollection services, String systemName)
        {
            var actorSystem = ActorSystem.Create(systemName);
            return services.AddEventFly(actorSystem);
        }

        public static EventFlyBuilder AddEventFly(this IServiceCollection services, ActorSystem actorSystem)
        {
            services.AddSingleton(actorSystem);

            services.AddTransient<ISerializedCommandPublisher, SerializedCommandPublisher>();
            services.AddTransient<ISerializedQueryExecutor, SerializedQueryExecutor>();

            services.AddTransient<ICommandBus, CommandToAggregateManagerBus>();
            services.AddTransient<IQueryProcessor, QueryToQueryHandlerProcessor>();
            services.AddTransient<EventFly.Jobs.IScheduler, JobToJobMannagerScheduler>();
            services.AddTransient<ICommandsScheduler, JobCommandsScheduler>();
            services.AddTransient<IEventsScheduler, JobEventsScheduler>();

            services.AddSingleton<ICommandValidator, DefaultCommandValidator>();
            services.AddSingleton<ICommandValidator, PermissionCommandValidator>();
            services.AddSingleton<ISecurityService, SecurityService>();

            services.AddSingleton<IDomainEventBus, EventStreamDomainEventBus>();
            services.AddSingleton<IDomainEventFactory, DomainEventFactory>();

            return new EventFlyBuilder(services);
        }

        public static IServiceCollection AddCommandHandler<TAggregate, TIdentity, TCommand, TCommandHandler>(this IServiceCollection services)
            where TAggregate : ActorBase, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
            where TCommand : ICommand<TIdentity>
            where TCommandHandler : CommandHandler<TAggregate, TIdentity, TCommand>
        {
            services.AddScoped<CommandHandler<TAggregate, TIdentity, TCommand>, TCommandHandler>();
            return services;
        }
    }
}