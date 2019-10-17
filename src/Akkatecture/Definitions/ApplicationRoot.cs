using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Commands.ExecutionResults;
using Akkatecture.Core;
using Akkatecture.Exceptions;
using Akkatecture.Extensions;
using Akkatecture.Queries;
using Akkatecture.ReadModels;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Definitions
{
    public interface IAggregateManagerDefinition
    {
        Type AggregateType { get; }
        Type IdentityType { get; }
    }

    internal sealed class AggregateManagerDefinition : IAggregateManagerDefinition
    {
        public AggregateManagerDefinition(Type aggregateType, Type identityType)
        {
            AggregateType = aggregateType;
            IdentityType = identityType;
        }

        public Type AggregateType { get; }

        public Type IdentityType { get; }
    }

    public interface ISagaManagerDefinition : IAggregateManagerDefinition
    {
        Type SagaLocatorType { get; }
    }

    internal sealed class AggregateSagaManagerDefinition : ISagaManagerDefinition
    {
        public AggregateSagaManagerDefinition(Type aggregateType, Type identityType, Type sagaLocatorType)
        {
            AggregateType = aggregateType;
            IdentityType = identityType;
            SagaLocatorType = sagaLocatorType;
        }

        public Type AggregateType { get; }
        public Type IdentityType { get; }
        public Type SagaLocatorType { get; }
    }

    public interface IQueryManagerDefinition
    {
        Type QueryHandlerType { get; }
        Type QueryType { get; }
        Type ResultType { get; }
    }

    internal sealed class QueryManagerDefinition : IQueryManagerDefinition
    {
        public QueryManagerDefinition(Type queryHandlerType, Type queryType, Type resultType)
        {
            QueryHandlerType = queryHandlerType;
            QueryType = queryType;
            ResultType = resultType;
        }

        public Type QueryHandlerType { get; }
        public Type QueryType { get; }
        public Type ResultType { get; }
    }

    internal sealed class ReadModelManagerDefinition : IReadModelManagerDefinition
    {
        public ReadModelManagerDefinition(Type readModelManagerType)
        {
            ReadModelManagerType = readModelManagerType;
        }

        public Type ReadModelManagerType { get; }
    }

    public interface IReadModelManagerDefinition
    {
        Type ReadModelManagerType { get; }
    }

    /// <summary>
    /// God object that runs everything
    /// </summary>
    internal sealed class ApplicationRoot : IApplicationRoot
    {
        private IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> DefinitionToAggregateManager { get; }
        private IReadOnlyDictionary<IQueryManagerDefinition, IActorRef> DefinitionToQueryManager { get; }
        private IReadOnlyDictionary<ISagaManagerDefinition, IActorRef> DefinitionToSagaManager { get; }
        private IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> DefinitionToReadModelManager { get; }


        public ApplicationRoot(ActorSystem actorSystem, IApplicationDefinition definitions)
        {
            Definitions = definitions;

            var dictionaryAggregate = new Dictionary<IAggregateManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions.Aggregates.Select(a => a.ManagerDefinition))
            {
                var type = typeof(AggregateManager<,>);
                var generics = type.MakeGenericType(new[] { managerDef.AggregateType, managerDef.IdentityType });

                var manager = actorSystem.ActorOf(
                    Props.Create(generics),
                    $"aggregate-{managerDef.AggregateType.GetAggregateName()}-manager"
                );
                dictionaryAggregate.Add(managerDef, manager);
            }

            DefinitionToAggregateManager = dictionaryAggregate;
            
            var dictionaryQuery = new Dictionary<IQueryManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions.Queries.Select(a => a.ManagerDefinition))
            {
                var type = typeof(QueryManager<,,>);
                var generics = type.MakeGenericType(new[] { managerDef.QueryHandlerType, managerDef.QueryType, managerDef.ResultType });

                var manager = actorSystem.ActorOf(Props.Create(generics),
                    $"query-{managerDef.QueryType.Name}-manager");

                dictionaryQuery.Add(managerDef, manager);
            }

            DefinitionToQueryManager = dictionaryQuery;

            var dictionarySaga = new Dictionary<ISagaManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions.Sagas.Select(a => a.ManagerDefinition))
            {
                var type = typeof(AggregateSagaManager<,,>);
                var generics = type.MakeGenericType(new[] { managerDef.AggregateType, managerDef.IdentityType, managerDef.SagaLocatorType });

                var manager = actorSystem.ActorOf(
                    Props.Create(generics),
                    $"saga-{managerDef.IdentityType.Name}-manager"
                );
                dictionarySaga.Add(managerDef, manager);
            }

            DefinitionToSagaManager = dictionarySaga;

            var dictionaryReadModel = new Dictionary<IReadModelManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions.ReadModels.Select(a => a.ManagerDefinition))
            {
                var manager = actorSystem.ActorOf(
                    Props.Create(managerDef.ReadModelManagerType),
                    $"saga-{managerDef.ReadModelManagerType.Name}-manager"
                );

                dictionaryReadModel.Add(managerDef, manager);
            }

            DefinitionToReadModelManager = dictionaryReadModel;
        }

        public IApplicationDefinition Definitions { get; }

        public Task<TExecutionResult> PublishAsync<TExecutionResult, TIdentity>(
          ICommand<TIdentity, TExecutionResult> command)
          where TExecutionResult : IExecutionResult
          where TIdentity : IIdentity
        {
            return GetAggregateManager(typeof(TIdentity)).Ask<TExecutionResult>(command, new TimeSpan?());
        }

        public Task<IExecutionResult> PublishAsync(ICommand command)
        {
            return GetAggregateManager(command.GetAggregateId().GetType()).Ask<IExecutionResult>(command, new TimeSpan?());
        }

        public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            return GetQueryManager(query.GetType()).Ask<TResult>(query, new TimeSpan?());
        }

        public Task<object> QueryAsync(IQuery query)
        {
            return GetQueryManager(query.GetType()).Ask(query, new TimeSpan?());
        }

        internal IActorRef GetAggregateManager(Type type)
        {
            var manager = DefinitionToAggregateManager.FirstOrDefault(i =>
            {
                var (k, v) = (i.Key, i.Value);
                if (!(k.AggregateType == type))
                    return k.IdentityType == type;
                return true;
            }).Value;
            if (manager == null)
                throw new InvalidOperationException("Aggregate " + type.PrettyPrint() + " not registered");
            return manager;
        }

        internal IActorRef GetQueryManager(Type queryType)
        {
            var manager = DefinitionToQueryManager.FirstOrDefault(i =>
            {
                var (k, v) = (i.Key, i.Value);

                return k.QueryType == queryType;
            }).Value;
            if (manager == null)
                throw new InvalidOperationException("Query " + queryType.PrettyPrint() + " not registered");
            return manager;
        }

        internal IActorRef GetSagaManager(Type type)
        {
            var manager = DefinitionToSagaManager.FirstOrDefault(i =>
            {
                var (k, v) = (i.Key, i.Value);

                if (!(k.AggregateType == type))
                    return k.IdentityType == type;
                return true;
            }).Value;
            if (manager == null)
                throw new InvalidOperationException("Saga " + type.PrettyPrint() + " not registered");
            return manager;
        }

        internal IActorRef GetReadModelManager(Type type)
        {
            var manager = DefinitionToReadModelManager.FirstOrDefault(i =>
            {
                var (k, v) = (i.Key, i.Value);

                return k.ReadModelManagerType == type;
            }).Value;
            if (manager == null)
                throw new InvalidOperationException("Saga " + type.PrettyPrint() + " not registered");
            return manager;
        }
    }
}
