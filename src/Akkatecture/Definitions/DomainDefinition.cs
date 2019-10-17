using System;
using System.Collections.Generic;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Extensions;
using Akkatecture.Jobs;
using Akkatecture.Queries;
using Akkatecture.ReadModels;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Definitions
{
    public abstract class DomainDefinition : IDomainDefinition
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
        public IDomainDependencies<IDomainDefinition> DomainDependencies { get; internal set; }

        protected IDomainDefinition RegisterAggregate<TAggregate, TIdentity>()
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

        protected IDomainDefinition RegisterQuery<TQueryHandler, TQuery, TResult>()
          where TQueryHandler : ActorBase, IQueryHandler<TQuery, TResult>
          where TQuery : IQuery<TResult>
        {
            //var manager = _system.ActorOf(Props.Create(() => new QueryManager<TQueryHandler, TQuery, TResult>()),
            //    $"query-{typeof(TQuery).Name}-manager");

            _queries.Add(
                new QueryDefinition(typeof(TQuery), typeof(TResult),
                    new QueryManagerDefinition(typeof(TQueryHandler), typeof(TQuery), typeof(TResult))
                )
            );
            return this;
        }

        protected IDomainDefinition RegisterSaga<TSaga, TSagaId, TSagaLocator>()
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

        protected IDomainDefinition RegisterSaga<TSaga, TSagaId>()
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

        protected IDomainDefinition RegisterReadModel<TReadModel, TReadModelManager>()
          where TReadModel : IReadModel
          where TReadModelManager : ActorBase, IReadModelManager, new()
        {
            //var manager = _system.ActorOf(Props.Create(() => new TReadModelManager()),
            //    $"read~model-{typeof(TReadModel).Name}-manager");

            _readModels.Add(
                new ReadModelDefinition(typeof(TReadModel),
                    new ReadModelManagerDefinition(typeof(TReadModelManager))
                )
            );
            return this;
        }

        protected IDomainDefinition RegisterAggregateReadModel<TReadModel, TIdentity>()
          where TReadModel : ActorBase, IReadModel<TIdentity>
          where TIdentity : IIdentity
        {
            return RegisterReadModel<TReadModel, AggregateReadModelManager<TReadModel, TIdentity>>();
        }

        protected IDomainDefinition RegisterCommand<TCommand>() where TCommand : ICommand
        {
            Commands.Load(typeof(TCommand));
            return this;
        }

        protected IDomainDefinition RegisterCommands(params Type[] types)
        {
            Commands.Load(types);
            return this;
        }

        protected IDomainDefinition RegisterEvent<TEvent>() where TEvent : IAggregateEvent
        {
            Events.Load(typeof(TEvent));
            return this;
        }

        protected IDomainDefinition RegisterEvents(params Type[] types)
        {
            Events.Load(types);
            return this;
        }

        protected IDomainDefinition RegisterSnapshot<TSnapshot>() where TSnapshot : IAggregateSnapshot
        {
            Snapshots.Load(typeof(TSnapshot));
            return this;
        }

        protected IDomainDefinition RegisterSnapshots(params Type[] types)
        {
            Snapshots.Load(types);
            return this;
        }

        protected IDomainDefinition RegisterJob<TJob>() where TJob : IJob
        {
            Jobs.Load(typeof(TJob));
            return this;
        }

        protected IDomainDefinition RegisterJobs(params Type[] types)
        {
            Jobs.Load(types);
            return this;
        }

    }
}
