using Akka.Actor;
using EventFly.Definitions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace EventFly.DependencyInjection
{
    public sealed class EventFlyBuilder
    {
        public IServiceCollection Services { get; }
        private readonly ActorSystem _actorSystem;
        private readonly ApplicationDefinition _applicationDefinition;
        public IApplicationDefinition ApplicationDefinition => _applicationDefinition;

        public EventFlyBuilder(ActorSystem actorSystem, IServiceCollection services)
        {
            _actorSystem = actorSystem;
            Services = services;
            _applicationDefinition = new ApplicationDefinition();
            Services.AddSingleton<IApplicationDefinition>(_applicationDefinition);
        }

        public EventFlyBuilder WithContext<TContext>()
            where TContext : ContextDefinition, new()
        {
            var context = new TContext();

            context.DI(Services);

            _applicationDefinition.RegisterContext(context);
            return this;
        }

        /// <summary>
        /// Will build EventFly and run actors based on definitions. Actor ran will be removed in the future.
        /// </summary>
        /// <returns></returns>
        public IServiceCollection BuildEventFly()
        {
            var registry = new DefinitionToManagerRegistryBuilder()
                .UseSystem(_actorSystem)
                    .RegisterAggregateManagers(ApplicationDefinition.Aggregates.Select(a => a.ManagerDefinition).ToList())
                    .RegisterQueryManagers(ApplicationDefinition.Queries.Select(a => a.ManagerDefinition).ToList())
                    .RegisterReadModelManagers(ApplicationDefinition.ReadModels.Select(a => a.ManagerDefinition).ToList())
                    .RegisterSagaManagers(ApplicationDefinition.Sagas.Select(a => a.ManagerDefinition).ToList())
                    .RegisterDomainServiceManagers(ApplicationDefinition.DomainServices.Select(a => a.ManagerDefinition).ToList())
                .Build();

            return Services.AddSingleton<IDefinitionToManagerRegistry>(registry);
        }
    }
}
