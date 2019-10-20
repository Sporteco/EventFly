using Akka.Actor;
using EventFly.Commands;
using EventFly.Definitions;
using EventFly.Queries;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace EventFly.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static EventFlyBuilder AddEventFly(
            this IServiceCollection services,
            ActorSystem actorSystem,
            Action<DomainsBuilder> domainsBuilderAction)
        {
            var domainsBuilder = new DomainsBuilder();

            domainsBuilderAction(
                domainsBuilder
            );

            var appDef = new ApplicationDefinition(domainsBuilder.Domains.ToList());

            foreach(var dep in domainsBuilder.Domains.Where(d => d.DomainDependencies != null).SelectMany(d => d.DomainDependencies.Dependencies))
                services.Add(dep);

            services
                .AddSingleton(actorSystem)
                .AddSingleton<IApplicationDefinition>(appDef);

            services.AddTransient<ISerializedCommandPublisher, SerializedCommandPublisher>();
            services.AddTransient<ISerializedQueryExecutor, SerializedQueryExecutor>();

            services.AddTransient<ICommandBus, CommandToAggregateManagerBus>();
            services.AddTransient<IQueryProcessor, QueryToQueryHandlerProcessor>();

            var registry = new DefinitionToManagerRegistryBuilder()
                .UseSystem(actorSystem)
                    .RegisterAggregateManagers(appDef.Aggregates.Select(a => a.ManagerDefinition).ToList())
                    .RegisterSagaManagers(appDef.Sagas.Select(a => a.ManagerDefinition).ToList())
                    .RegisterQueryManagers(appDef.Queries.Select(a => a.ManagerDefinition).ToList())
                    .RegisterReadModelManagers(appDef.ReadModels.Select(a => a.ManagerDefinition).ToList())
                .Build();

            services
                .AddSingleton<IDefinitionToManagerRegistry>(registry);

            return new EventFlyBuilder(appDef, services);
        }
    }
}
