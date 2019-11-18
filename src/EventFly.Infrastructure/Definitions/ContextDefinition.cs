using Akka.Actor;
using EventFly.Aggregates;
using EventFly.Aggregates.Snapshot;
using EventFly.Commands;
using EventFly.Core;
using EventFly.DomainService;
using EventFly.Jobs;
using EventFly.Permissions;
using EventFly.Queries;
using EventFly.ReadModels;
using EventFly.Sagas;
using EventFly.Sagas.AggregateSaga;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace EventFly.Definitions
{
    public abstract class ContextDefinition : IContextDefinition
    {
        public String Name => GetType().Name;
        public IReadOnlyCollection<IAggregateDefinition> Aggregates => _aggregates;
        public IReadOnlyCollection<ISagaDefinition> Sagas => _sagas;
        public IReadOnlyCollection<IDomainServiceDefinition> DomainServices => _services;
        public IReadOnlyCollection<IReadModelDefinition> ReadModels => _readModels;
        public IReadOnlyCollection<IQueryDefinition> Queries => _queries;
        public IReadOnlyCollection<IJobDefinition> Jobs => _jobs;
        public IReadOnlyCollection<IPermissionDefinition> Permissions => _permissions;
        public EventDefinitions Events { get; } = new EventDefinitions();
        public CommandDefinitions Commands { get; } = new CommandDefinitions();
        public SnapshotDefinitions Snapshots { get; } = new SnapshotDefinitions();

        public abstract IServiceCollection DI(IServiceCollection serviceDescriptors);

        protected IContextDefinition RegisterAggregate<TAggregate, TIdentity>()
            where TAggregate : ActorBase, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var manager = new AggregateManagerDefinition(typeof(TAggregate), typeof(TIdentity));
            _aggregates.Add(new AggregateDefinition(typeof(TAggregate), typeof(TIdentity), manager));
            return this;
        }

        protected IContextDefinition RegisterQuery<TQuery, TResult>()
            where TQuery : IQuery<TResult>
        {
            var manager = new QueryManagerDefinition(typeof(QueryHandler<TQuery, TResult>), typeof(TQuery), typeof(TResult));
            _queries.Add(new QueryDefinition(typeof(TQuery), typeof(TResult), manager));
            return this;
        }

        protected IContextDefinition RegisterSaga<TSaga, TSagaId, TSagaLocator>()
            where TSaga : ActorBase, IAggregateSaga<TSagaId>
            where TSagaId : IIdentity
            where TSagaLocator : class, ISagaLocator<TSagaId>, new()
        {
            var manager = new AggregateSagaManagerDefinition(typeof(TSaga), typeof(TSagaId), typeof(TSagaLocator));
            _sagas.Add(new SagaDefinition(typeof(TSaga), typeof(TSagaId), manager));
            return this;
        }

        protected IContextDefinition RegisterDomainService<TService>()
            where TService : ActorBase, IDomainService
        {
            var manager = new DomainServiceManagerDefinition(typeof(TService));
            _services.Add(new DomainServiceDefinition(typeof(TService), manager));
            return this;
        }

        protected IContextDefinition RegisterSaga<TSaga, TSagaId>()
            where TSaga : ActorBase, IAggregateSaga<TSagaId>
            where TSagaId : class, IIdentity
        {
            var manager = new AggregateSagaManagerDefinition(typeof(TSaga), typeof(TSagaId), typeof(SagaLocatorByIdentity<TSagaId>));
            _sagas.Add(new SagaDefinition(typeof(TSaga), typeof(TSagaId), manager));
            return this;
        }

        protected IContextDefinition RegisterJob<TJob, TJobId, TJobRunner, TJobScheduler>()
            where TJob : IJob<TJobId>
            where TJobId : IJobId
            where TJobRunner : JobRunner<TJob, TJobId>
            where TJobScheduler : JobScheduler<TJobScheduler, TJob, TJobId>
        {
            var manager = new JobManagerDefinition(typeof(TJobRunner), typeof(TJobScheduler), typeof(TJob), typeof(TJobId));
            _jobs.Add(new JobDefinition(typeof(TJob), typeof(TJobId), manager));
            return this;
        }

        protected IContextDefinition RegisterPermission(String permissionCode)
        {
            var permissionDef = new PermissionDefinition(permissionCode);
            if (!_permissions.Contains(permissionDef)) _permissions.Add(permissionDef);
            return this;
        }

        protected IContextDefinition RegisterPermission<TIdentity>(String permissionCode)
            where TIdentity : IIdentity
        {
            var permissionDef = new PermissionDefinition(typeof(TIdentity), permissionCode);
            if (!_permissions.Contains(permissionDef)) _permissions.Add(permissionDef);
            return this;
        }

        protected IContextDefinition RegisterReadModel<TReadModel, TReadModelManager>()
            where TReadModel : IReadModel
            where TReadModelManager : ActorBase, IReadModelManager, new()
        {
            var manager = new ReadModelManagerDefinition(typeof(TReadModelManager), typeof(TReadModel));
            _readModels.Add(new ReadModelDefinition(typeof(TReadModel), manager));
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

        private readonly List<IAggregateDefinition> _aggregates = new List<IAggregateDefinition>();
        private readonly List<IDomainServiceDefinition> _services = new List<IDomainServiceDefinition>();
        private readonly List<ISagaDefinition> _sagas = new List<ISagaDefinition>();
        private readonly List<IReadModelDefinition> _readModels = new List<IReadModelDefinition>();
        private readonly List<IQueryDefinition> _queries = new List<IQueryDefinition>();
        private readonly List<IJobDefinition> _jobs = new List<IJobDefinition>();
        private readonly List<IPermissionDefinition> _permissions = new List<IPermissionDefinition>();
    }
}