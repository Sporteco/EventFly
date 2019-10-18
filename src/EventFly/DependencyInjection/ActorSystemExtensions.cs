using Akka.Actor;
using Akka.DI.Core;
using EventFly.Commands;
using EventFly.Definitions;
using EventFly.Queries;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventFly.DependencyInjection
{
    public class ServiceProviderExtension : ExtensionIdProvider<ServiceProviderHolder>
    {
        private readonly ServiceProviderHolder _serviceProviderHolder;

        public ServiceProviderExtension(ServiceProviderHolder serviceProviderHolder)
        {
            _serviceProviderHolder = serviceProviderHolder;
        }

        public override ServiceProviderHolder CreateExtension(ExtendedActorSystem system)
        {
            return _serviceProviderHolder;
        }
    }
    public class ServiceProviderHolder : IExtension
    {
        public ServiceProviderHolder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

    }

    public sealed class DomainsBuilder
    {
        public sealed class DomainBuilder<TDomain>
            where TDomain : IDomainDefinition
        {
            private readonly TDomain _domain;
            public DomainBuilder(TDomain domain)
            {
                _domain = domain;
            }

            public DomainBuilder<TDomain> WithDependencies<TDomainDependencies>()
                where TDomainDependencies : IDomainDependencies<IDomainDefinition>, new()
            {
                var deps = new TDomainDependencies();

                if (_domain is DomainDefinition assignableDomain)
                    assignableDomain.DomainDependencies = deps;

                return this;
            }
        }

        public IList<IDomainDefinition> Domains { get; } = new List<IDomainDefinition>();

        public DomainBuilder<TDomain> RegisterDomainDefinitions<TDomain>()
            where TDomain : IDomainDefinition, new()
        {
            var domain = new TDomain();
            Domains.Add(domain);
            return new DomainBuilder<TDomain>(domain);
        }
    }

    public static class ActorSystemExtensions
    {
        public static ActorSystem RegisterDependencyResolver(
            this ActorSystem actorSystem,
            IServiceProvider serviceProvider)
        {
            var factoryScope = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            var resolver = new MicrosoftDependencyResolver(factoryScope, actorSystem);
            actorSystem.AddDependencyResolver(resolver);

            actorSystem.RegisterExtension(new ServiceProviderExtension(new ServiceProviderHolder(serviceProvider)));
            return actorSystem;
        }

        public static IServiceCollection AddEventFly(
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

            return services;
        }
    }
}
