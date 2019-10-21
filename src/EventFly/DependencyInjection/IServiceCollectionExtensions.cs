using Akka.Actor;
using EventFly.Commands;
using EventFly.Core;
using EventFly.Definitions;
using EventFly.Jobs;
using EventFly.Queries;
using EventFly.ReadModels;
using EventFly.Sagas;
using EventFly.Sagas.AggregateSaga;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventFly.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static EventFlyBuilder AddEventFly(
            this IServiceCollection services,
            ActorSystem actorSystem,
            Action<InfrastructureBuilder> infrastructureBuilderAction,
            Action<DomainsBuilder> domainsBuilderAction)
        {
            var domainsBuilder = new DomainsBuilder();
            var infrasrtuctureBuilder = new InfrastructureBuilder();

            infrastructureBuilderAction(
                infrasrtuctureBuilder
            );

            domainsBuilderAction(
                domainsBuilder
            );

            var appDef = new ApplicationDefinition(
                domainsBuilder.Domains.ToList(),
                infrasrtuctureBuilder.ReadModels,
                infrasrtuctureBuilder.Sagas,
                infrasrtuctureBuilder.Jobs
            );

            foreach (var dep in domainsBuilder.Domains.Where(d => d.DomainDependencies != null).SelectMany(d => d.DomainDependencies.Dependencies))
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
                    .RegisterQueryManagers(appDef.Queries.Select(a => a.ManagerDefinition).ToList())
                    .RegisterReadModelManagers(appDef.ReadModels.Select(a => a.ManagerDefinition).ToList())
                    .RegisterSagaManagers(appDef.Sagas.Select(a => a.ManagerDefinition).ToList())
                .Build();

            services
                .AddSingleton<IDefinitionToManagerRegistry>(registry);

            return new EventFlyBuilder(appDef, services);
        }
    }

    public sealed class InfrastructureBuilder
    {
        private readonly List<IReadModelDefinition> _readModelDefinitions = new List<IReadModelDefinition>();
        private readonly List<ISagaDefinition> _sagaDefinitions = new List<ISagaDefinition>();

        public IReadOnlyCollection<IReadModelDefinition> ReadModels => _readModelDefinitions;
        public IReadOnlyCollection<ISagaDefinition> Sagas => _sagaDefinitions;
        public JobDefinitions Jobs { get; } = new JobDefinitions();

        public InfrastructureBuilder RegisterInfrastructureDefinitions<TInfrastructureDefinitions>()
            where TInfrastructureDefinitions : IInfrastructureDefinitions, new()
        {
            var definitions = new TInfrastructureDefinitions();

            definitions.Describe(this);
            return this;
        }

        public InfrastructureBuilder AddReadModel<TReadModel, TReadModelManager>()
            where TReadModel : IReadModel
            where TReadModelManager : ActorBase, IReadModelManager, new()
        {
            _readModelDefinitions.Add(
                new ReadModelDefinition(typeof(TReadModel),
                    new ReadModelManagerDefinition(typeof(TReadModelManager), typeof(TReadModel))
                ));
            return this;
        }

        public InfrastructureBuilder AddJob<TJob>() where TJob : IJob
        {
            Jobs.Load(typeof(TJob));
            return this;
        }

        public InfrastructureBuilder AddJobs(params Type[] types)
        {
            Jobs.Load(types);
            return this;
        }

        public InfrastructureBuilder AddAggregateReadModel<TReadModel, TIdentity>()
            where TReadModel : ReadModel, new()
            where TIdentity : IIdentity
        {
            return AddReadModel<TReadModel, AggregateReadModelManager<TReadModel, TIdentity>>();
        }

        public InfrastructureBuilder AddSaga<TSaga, TSagaId, TSagaLocator>()
            where TSaga : ActorBase, IAggregateSaga<TSagaId>
            where TSagaId : IIdentity
            where TSagaLocator : class, ISagaLocator<TSagaId>, new()
        {
            _sagaDefinitions.Add(
                new SagaDefinition(typeof(TSaga), typeof(TSagaId),
                    new AggregateSagaManagerDefinition(typeof(TSaga), typeof(TSagaId), typeof(TSagaLocator))
                )
            );

            return this;
        }

        public InfrastructureBuilder AddSaga<TSaga, TSagaId>()
            where TSaga : ActorBase, IAggregateSaga<TSagaId>
            where TSagaId : IIdentity
        {
            _sagaDefinitions.Add(
                new SagaDefinition(typeof(TSaga), typeof(TSagaId),
                    new AggregateSagaManagerDefinition(typeof(TSaga), typeof(TSagaId), typeof(SagaLocatorByIdentity<TSagaId>))
                )
            );

            return this;
        }
    }
}
