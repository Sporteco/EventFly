using System;
using System.Collections.Generic;
using Akka.Actor;
using EventFly.Aggregates;
using EventFly.Aggregates.Snapshot;
using EventFly.Commands;
using EventFly.Core;
using EventFly.Jobs;
using EventFly.Queries;
using EventFly.ReadModels;
using EventFly.Sagas;
using EventFly.Sagas.AggregateSaga;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.Definitions
{
    public abstract class ContextDefinition : IContextDefinition
    {
        private readonly List<IAggregateDefinition> _aggregates = new List<IAggregateDefinition>();
        private readonly List<ISagaDefinition> _sagas = new List<ISagaDefinition>();
        private readonly List<IReadModelDefinition> _readModels = new List<IReadModelDefinition>();
        private readonly List<IQueryDefinition> _queries = new List<IQueryDefinition>();

        public string Name => GetType().Name;

        public IReadOnlyCollection<IAggregateDefinition> Aggregates => _aggregates;

        public IReadOnlyCollection<ISagaDefinition> Sagas => _sagas;

        public IReadOnlyCollection<IReadModelDefinition> ReadModels => _readModels;

        public IReadOnlyCollection<IQueryDefinition> Queries => _queries;

        public EventDefinitions Events { get; } = new EventDefinitions();

        public CommandDefinitions Commands { get; } = new CommandDefinitions();

        public SnapshotDefinitions Snapshots { get; } = new SnapshotDefinitions();


        public JobDefinitions Jobs { get; } = new JobDefinitions();

        protected IContextDefinition RegisterAggregate<TAggregate, TIdentity>()
          where TAggregate : ActorBase, IAggregateRoot<TIdentity>
          where TIdentity : IIdentity
        {
            //var manager = _system.ActorOf(Props.Create(() => new AggregateManager<TAggregate, TIdentity>()),
            //    $"aggregate-{typeof(TAggregate).GetAggregateName()}-manager");

            _aggregates.Add(
                new AggregateDefinition(typeof(TAggregate), typeof(TIdentity),
                    new AggregateManagerDefinition(typeof(TAggregate), typeof(TIdentity))
                )
            );
            return this;
        }

        public abstract IServiceCollection DI(IServiceCollection serviceDescriptors);

        protected IContextDefinition RegisterQuery<TQuery, TResult>()
          where TQuery : IQuery<TResult>
        {
            //var manager = _system.ActorOf(Props.Create(() => new QueryManager<TQueryHandler, TQuery, TResult>()),
            //    $"query-{typeof(TQuery).Name}-manager");

            _queries.Add(
                new QueryDefinition(typeof(TQuery), typeof(TResult),
                    new QueryManagerDefinition(typeof(QueryHandler<TQuery, TResult>), typeof(TQuery), typeof(TResult))
                )
            );
            return this;
        }

        protected IContextDefinition RegisterSaga<TSaga, TSagaId, TSagaLocator>()
          where TSaga : ActorBase, IAggregateSaga<TSagaId>
          where TSagaId : IIdentity
          where TSagaLocator : class, ISagaLocator<TSagaId>, new()
        {
            //var manager = _system.ActorOf(Props.Create(() => new AggregateSagaManager<TSaga, TSagaId, TSagaLocator>()),
            //    $"saga-{typeof(TSagaId).Name}-manager");

            _sagas.Add(
                new SagaDefinition(typeof(TSaga), typeof(TSagaId),
                    new AggregateSagaManagerDefinition(typeof(TSaga), typeof(TSagaId), typeof(TSagaLocator))
                )
            );
            return this;
        }

        protected IContextDefinition RegisterSaga<TSaga, TSagaId>()
          where TSaga : ActorBase, IAggregateSaga<TSagaId>
          where TSagaId : IIdentity
        {
            //var manager = _system.ActorOf(Props.Create(() => new AggregateSagaManager<TSaga, TSagaId, SagaLocatorByIdentity<TSagaId>>()),
            //    $"saga-{typeof(TSagaId).Name}-manager");

            _sagas.Add(
                new SagaDefinition(typeof(TSaga), typeof(TSagaId),
                    new AggregateSagaManagerDefinition(typeof(TSaga), typeof(TSagaId), typeof(SagaLocatorByIdentity<TSagaId>))
                )
            );
            return this;
        }

        protected IContextDefinition RegisterReadModel<TReadModel, TReadModelManager>()
          where TReadModel : IReadModel
          where TReadModelManager : ActorBase, IReadModelManager, new()
        {
            //var manager = _system.ActorOf(Props.Create(() => new TReadModelManager()),
            //    $"read~model-{typeof(TReadModel).Name}-manager");

            _readModels.Add(
                new ReadModelDefinition(typeof(TReadModel),
                    new ReadModelManagerDefinition(typeof(TReadModelManager), typeof(TReadModel))
                )
            );
            return this;
        }

        protected IContextDefinition RegisterAggregateReadModel<TReadModel, TIdentity>()
          where TReadModel : ReadModel, new()
            where TIdentity : IIdentity
        {
            return RegisterReadModel<TReadModel, AggregateReadModelManager<TReadModel, TIdentity>>();
        }


        protected IContextDefinition RegisterCommand<TCommand>() where TCommand : ICommand
        {
            Commands.Load(typeof(TCommand));
            return this;
        }

        protected IContextDefinition RegisterCommands(params Type[] types)
        {
            Commands.Load(types);
            return this;
        }

        protected IContextDefinition RegisterEvent<TEvent>() where TEvent : IAggregateEvent
        {
            Events.Load(typeof(TEvent));
            return this;
        }

        protected IContextDefinition RegisterEvents(params Type[] types)
        {
            Events.Load(types);
            return this;
        }

        protected IContextDefinition RegisterSnapshot<TSnapshot>() where TSnapshot : IAggregateSnapshot
        {
            Snapshots.Load(typeof(TSnapshot));
            return this;
        }

        protected IContextDefinition RegisterSnapshots(params Type[] types)
        {
            Snapshots.Load(types);
            return this;
        }

        protected IContextDefinition RegisterJob<TJob>() where TJob : IJob
        {
            Jobs.Load(typeof(TJob));
            return this;
        }

        protected IContextDefinition RegisterJobs(params Type[] types)
        {
            Jobs.Load(types);
            return this;
        }

    }
}
