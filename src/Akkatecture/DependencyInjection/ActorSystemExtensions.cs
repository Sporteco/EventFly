using Akka.Actor;
using Akka.DI.Core;
using Akkatecture.Commands;
using Akkatecture.Definitions;
using Akkatecture.Queries;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Akkatecture.DependencyInjection
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
            private readonly IServiceCollection _services;

            public DomainBuilder(TDomain domain, IServiceCollection services)
            {
                _domain = domain;
                _services = services;
            }

            public DomainBuilder<TDomain> WithDependencies<TDomainDependencies>()
                where TDomainDependencies : IDomainDependencies<IDomainDefinition>, new()
            {
                var deps = new TDomainDependencies();

                if (_domain is DomainDefinition assignableDomain)
                    assignableDomain.DomainDependencies = deps;

                foreach (var dep in deps.Dependencies)
                    _services.Add(dep);

                return this;
            }
        }

        public DomainsBuilder(IApplicationDefinition applicationDefinition, IServiceCollection services)
        {
            ApplicationDefinition = applicationDefinition;
            Services = services;
        }

        public IApplicationDefinition ApplicationDefinition { get; }

        public IServiceCollection Services { get; }

        public DomainBuilder<TDomain> RegisterDomainDefinitions<TDomain>()
            where TDomain : IDomainDefinition
        {
            return new DomainBuilder<TDomain>(ApplicationDefinition.RegisterDomainDefenitions<TDomain>(), Services);
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

        public static IServiceCollection AddAkkatecture(
            this IServiceCollection services,
            ActorSystem actorSystem,
            Action<DomainsBuilder> domainsBuilder)
        {
            var definitions = new ApplicationDefinition(actorSystem);


            services
                .AddSingleton(actorSystem)
                .AddSingleton<IApplicationDefinition>(definitions);

            services.AddTransient<ISerializedCommandPublisher, SerializedCommandPublisher>();
            services.AddTransient<ISerializedQueryExecutor, SerializedQueryExecutor>();

            domainsBuilder(
                new DomainsBuilder(definitions, services)
            );

            var app = new ApplicationRoot(actorSystem, definitions: definitions);
            services
                .AddSingleton<IApplicationRoot>(app)
                .AddSingleton(app);

            return services;
        }
    }
}
