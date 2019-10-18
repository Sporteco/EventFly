using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Commands;
using Akkatecture.Commands.ExecutionResults;
using Akkatecture.Core;
using Akkatecture.Exceptions;
using Akkatecture.Queries;
using Akkatecture.ReadModels;

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
