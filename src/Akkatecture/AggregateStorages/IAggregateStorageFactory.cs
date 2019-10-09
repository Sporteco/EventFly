using Akkatecture.Aggregates;

namespace Akkatecture.AggregateStorages
{
    public interface IAggregateStorageFactory
    {
        IAggregateStorage GetAggregateStorage<TAggregate>()
            where TAggregate : IAggregateRoot;

        void RegisterStorage<TAggregate, TAggregateStorage>()
            where TAggregate : IAggregateRoot
            where TAggregateStorage : IAggregateStorage;

        void RegisterDefaultStorage<TAggregateStorage>()
            where TAggregateStorage : IAggregateStorage;
    }
}