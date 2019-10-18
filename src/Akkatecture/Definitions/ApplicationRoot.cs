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

    public interface IDefinitionToManagerRegistry
    {
        IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> DefinitionToAggregateManager { get; }
        IReadOnlyDictionary<IQueryManagerDefinition, IActorRef> DefinitionToQueryManager { get; }
        IReadOnlyDictionary<ISagaManagerDefinition, IActorRef> DefinitionToSagaManager { get; }
        IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> DefinitionToReadModelManager { get; }
    }

    public sealed class DefinitionToManagerRegistryBuilder
    {
        private ActorSystem System;

        private IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> DefinitionToAggregateManager { get; set; } = new Dictionary<IAggregateManagerDefinition, IActorRef>();
        private IReadOnlyDictionary<IQueryManagerDefinition, IActorRef> DefinitionToQueryManager { get; set; } = new Dictionary<IQueryManagerDefinition, IActorRef>();
        private IReadOnlyDictionary<ISagaManagerDefinition, IActorRef> DefinitionToSagaManager { get; set; } = new Dictionary<ISagaManagerDefinition, IActorRef>();
        private IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> DefinitionToReadModelManager { get; set; } = new Dictionary<IReadModelManagerDefinition, IActorRef>();

        public DefinitionToManagerRegistryBuilder UseSystem(ActorSystem actorSystem)
        {
            System = actorSystem;

            return this;
        }
        public DefinitionToManagerRegistryBuilder RegisterAggregateManagers(IReadOnlyCollection<IAggregateManagerDefinition> definitions)
        {
            var dictionaryAggregate = new Dictionary<IAggregateManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var type = typeof(AggregateManager<,>);
                var generics = type.MakeGenericType(new[] { managerDef.AggregateType, managerDef.IdentityType });

                var manager = System.ActorOf(
                    Props.Create(generics),
                    $"aggregate-{managerDef.AggregateType.GetAggregateName()}-manager"
                );
                dictionaryAggregate.Add(managerDef, manager);
            }

            DefinitionToAggregateManager = dictionaryAggregate;
            return this;
        }

        public DefinitionToManagerRegistryBuilder RegisterQueryManagers(IReadOnlyCollection<IQueryManagerDefinition> definitions)
        {
            var dictionaryQuery = new Dictionary<IQueryManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var type = typeof(QueryManager<,,>);
                var generics = type.MakeGenericType(new[] { managerDef.QueryHandlerType, managerDef.QueryType, managerDef.ResultType });

                var manager = System.ActorOf(Props.Create(generics),
                    $"query-{managerDef.QueryType.Name}-manager");

                dictionaryQuery.Add(managerDef, manager);
            }

            DefinitionToQueryManager = dictionaryQuery;

            return this;
        }

        public DefinitionToManagerRegistryBuilder RegisterSagaManagers(IReadOnlyCollection<ISagaManagerDefinition> definitions)
        {
            var dictionarySaga = new Dictionary<ISagaManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var type = typeof(AggregateSagaManager<,,>);
                var generics = type.MakeGenericType(new[] { managerDef.AggregateType, managerDef.IdentityType, managerDef.SagaLocatorType });

                var manager = System.ActorOf(
                    Props.Create(generics),
                    $"saga-{managerDef.IdentityType.Name}-manager"
                );
                dictionarySaga.Add(managerDef, manager);
            }
            DefinitionToSagaManager = dictionarySaga;
            return this;
        }

        public DefinitionToManagerRegistryBuilder RegisterReadModelManagers(IReadOnlyCollection<IReadModelManagerDefinition> definitions)
        {
            var dictionaryReadModel = new Dictionary<IReadModelManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var manager = System.ActorOf(
                    Props.Create(managerDef.ReadModelManagerType),
                    $"saga-{managerDef.ReadModelManagerType.Name}-manager"
                );

                dictionaryReadModel.Add(managerDef, manager);
            }
            DefinitionToReadModelManager = dictionaryReadModel;
            return this;
        }

        public DefinitionToManagerRegistry Build()
        {
            return new DefinitionToManagerRegistry(DefinitionToAggregateManager, DefinitionToQueryManager, DefinitionToSagaManager, DefinitionToReadModelManager);
        }
    }

    public sealed class DefinitionToManagerRegistry : IDefinitionToManagerRegistry
    {
        public DefinitionToManagerRegistry(
            IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> definitionToAggregateManager,
            IReadOnlyDictionary<IQueryManagerDefinition, IActorRef> definitionToQueryManager,
            IReadOnlyDictionary<ISagaManagerDefinition, IActorRef> definitionToSagaManager,
            IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> definitionToReadModelManager
        )
        {
            DefinitionToAggregateManager = definitionToAggregateManager;
            DefinitionToQueryManager = definitionToQueryManager;
            DefinitionToSagaManager = definitionToSagaManager;
            DefinitionToReadModelManager = definitionToReadModelManager;
        }

        public IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> DefinitionToAggregateManager { get; }
        public IReadOnlyDictionary<IQueryManagerDefinition, IActorRef> DefinitionToQueryManager { get; }
        public IReadOnlyDictionary<ISagaManagerDefinition, IActorRef> DefinitionToSagaManager { get; }
        public IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> DefinitionToReadModelManager { get; }
    }

    /// <summary>
    /// God object that runs everything
    /// </summary>
    internal sealed class ApplicationRoot : IApplicationRoot
    {
        private readonly IDefinitionToManagerRegistry _toManagerRegistry;

        public ApplicationRoot(IDefinitionToManagerRegistry toManagerRegistry)
        {
            _toManagerRegistry = toManagerRegistry;
        }

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
            var manager = _toManagerRegistry.DefinitionToAggregateManager.FirstOrDefault(i =>
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
            var manager = _toManagerRegistry.DefinitionToQueryManager.FirstOrDefault(i =>
            {
                var (k, v) = (i.Key, i.Value);

                return k.QueryType == queryType;
            }).Value;
            if (manager == null)
                throw new InvalidOperationException("Query " + queryType.PrettyPrint() + " not registered");
            return manager;
        }
    }
}
