﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Commands.ExecutionResults;
using Akkatecture.Core;
using Akkatecture.Exceptions;
using Akkatecture.Queries;
using Akkatecture.ReadModels;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture
{
    public static class SystemHostExtensions
    {
        private static readonly ConcurrentDictionary<int,SystemHost> _hostmap = new ConcurrentDictionary<int, SystemHost>();
        private static SystemHost GetInstance(ActorSystem system)
        {
            return _hostmap.GetOrAdd(system.GetHashCode(), new SystemHost(system));
        }

        public static SystemHost RegisterReadModel<TReadModel, TReadModelManager>(this ActorSystem system)
            where TReadModelManager : ActorBase, IReadModelManager, new() where TReadModel : IReadModel
            => GetInstance(system).RegisterReadModel<TReadModel, TReadModelManager>();

        public static SystemHost RegisterAggregateReadModel<TReadModel, TIdentity>(ActorSystem system)
            where TReadModel : ActorBase, IReadModel<TIdentity>
            where TIdentity : IIdentity
            => system.RegisterReadModel<TReadModel, AggregateReadModelManager<TReadModel, TIdentity>>();


        public static SystemHost RegisterAggregate<TAggregate, TIdentity>(this ActorSystem system) 
            where TAggregate : ActorBase, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
            => GetInstance(system).RegisterAggregate<TAggregate, TIdentity>();

        public static SystemHost RegisterSaga<TSaga, TSagaId, TSagaLocator>(this ActorSystem system)
            where TSaga : ActorBase, IAggregateSaga<TSagaId>
            where TSagaId : IIdentity
            where TSagaLocator : class, ISagaLocator<TSagaId>, new()
            => GetInstance(system).RegisterSaga<TSaga, TSagaId, TSagaLocator>();

        public static SystemHost RegisterSaga<TSaga, TSagaId>(this ActorSystem system)
            where TSaga : ActorBase, IAggregateSaga<TSagaId>
            where TSagaId : IIdentity
            => GetInstance(system).RegisterSaga<TSaga, TSagaId, SagaLocatorByIdentity<TSagaId>>();

        public static SystemHost RegisterQuery<TQueryHandler, TQuery, TResult>(this ActorSystem system)
            where TQueryHandler : ActorBase, IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
            => GetInstance(system).RegisterQuery<TQueryHandler, TQuery, TResult>();

        public static Task<TExecutionResult> PublishCommandAsync<TIdentity, TExecutionResult>(this ActorSystem system, ICommand<TIdentity, TExecutionResult> command)
            where TIdentity : IIdentity where TExecutionResult : IExecutionResult
            => GetInstance(system).PublishAsync(command);

        public static Task<TResult> ExecuteQueryAsync<TResult>(this ActorSystem system, IQuery<TResult> query)
            => GetInstance(system).QueryAsync(query);

        public static IActorRef GetAggregateManager<TIdentity>(this ActorSystem system) 
            where TIdentity : IIdentity
            => GetInstance(system).GetAggregateManager<TIdentity>();

        public static IActorRef GetSagaManager<TSagaId>(this ActorSystem system) 
            where TSagaId : IIdentity
            => GetInstance(system).GetSagaManager<TSagaId>();

        
    }

    public class SystemHost
    {
        private readonly ActorSystem _system;
        private readonly ConcurrentDictionary<Type,IActorRef> _dicIdentity2AggregateStorage = new ConcurrentDictionary<Type, IActorRef>();
        private readonly ConcurrentDictionary<Type,IActorRef> _dicIdentity2SagaStorage = new ConcurrentDictionary<Type, IActorRef>();
        private readonly ConcurrentDictionary<Type,IActorRef> _dicIdentity2QueryStorage = new ConcurrentDictionary<Type, IActorRef>();

        internal SystemHost(ActorSystem system)
        {
            _system = system;
        }

        public IActorRef GetSagaManager<TSagaId>() where TSagaId : IIdentity
        {
            if (!_dicIdentity2SagaStorage.TryGetValue(typeof(TSagaId), out var manager))
                throw new InvalidOperationException($"Saga [{typeof(TSagaId).PrettyPrint()}] not registered.");
            return manager;
        }

        public SystemHost RegisterReadModel<TReadModel,TReadModelManager>()
            where TReadModelManager : ActorBase, IReadModelManager, new() where TReadModel : IReadModel
        {
            _system.ActorOf(Props.Create(() =>
                new TReadModelManager()), $"read-model-{typeof(TReadModel).Name}-manager");
            return this;
        }
        public SystemHost RegisterAggregateReadModel<TReadModel, TIdentity>()
            where TReadModel : ActorBase, IReadModel<TIdentity>
            where TIdentity : IIdentity
            => RegisterReadModel<TReadModel, AggregateReadModelManager<TReadModel, TIdentity>>();

        public SystemHost RegisterAggregate<TAggregate,TIdentity>()
            where TAggregate : ActorBase, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var manager = _system.ActorOf(Props.Create(() =>
                new AggregateManager<TAggregate, TIdentity>()), $"aggregate-{typeof(TAggregate).Name}-manager");
            _dicIdentity2AggregateStorage[typeof(TIdentity)] = manager;
            return this;
        }

        public SystemHost RegisterSaga<TSaga,TSagaId,TSagaLocator>()
            where TSaga : ActorBase, IAggregateSaga<TSagaId>
            where TSagaId : IIdentity
            where TSagaLocator : class, ISagaLocator<TSagaId>, new()
        {
            var manager = _system.ActorOf(Props.Create(() =>
                new AggregateSagaManager<TSaga, TSagaId,TSagaLocator>()), $"saga-{typeof(TSagaId).Name}-manager");
            _dicIdentity2SagaStorage[typeof(TSagaId)] = manager;
            return this;
        }

        public SystemHost RegisterSaga<TSaga,TSagaId>()
            where TSaga : ActorBase, IAggregateSaga<TSagaId>
            where TSagaId : IIdentity
        {
            var manager = _system.ActorOf(Props.Create(() =>
                new AggregateSagaManager<TSaga, TSagaId,SagaLocatorByIdentity<TSagaId>>()), $"saga-{typeof(TSagaId).Name}-manager");
            _dicIdentity2SagaStorage[typeof(TSagaId)] = manager;
            return this;
        }

        public SystemHost RegisterQuery<TQueryHandler, TQuery, TResult>() 
            where TQueryHandler : ActorBase, IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
        {
            var manager = _system.ActorOf(Props.Create(() =>
                new QueryManager<TQueryHandler, TQuery, TResult>()), $"query-{typeof(TQuery).Name}-manager");
            _dicIdentity2QueryStorage[typeof(TQuery)] = manager;
            return this;
        }
        public Task<TExecutionResult> PublishAsync<TIdentity, TExecutionResult>(ICommand<TIdentity, TExecutionResult> command) 
            where TIdentity : IIdentity where TExecutionResult : IExecutionResult 
            => GetAggregateManager<TIdentity>().Ask<TExecutionResult>(command);

        public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            var type = query.GetType();
            if (!_dicIdentity2QueryStorage.TryGetValue(type, out var manager))
                throw new InvalidOperationException($"Query [{type.PrettyPrint()}] not registered.");
            return manager.Ask<TResult>(query);
        }

        public IActorRef GetAggregateManager<TIdentity>() where TIdentity : IIdentity
        {
            if (!_dicIdentity2AggregateStorage.TryGetValue(typeof(TIdentity), out var manager))
                throw new InvalidOperationException($"Aggregate [{typeof(TIdentity).PrettyPrint()}] not registered.");
            return manager;
        }
    }
}