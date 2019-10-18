using System;

namespace EventFly.Definitions
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
}
