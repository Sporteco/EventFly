using Akkatecture.Aggregates;

namespace Akkatecture.AggregateStorages
{
    public interface IAggregateStorageFactory
    {
        IAggregateStorage GetAggregateStorage<TAggregate>()
            where TAggregate : IAggregateRoot;

        void RegisterStorage<TAggregate>(IAggregateStorage storage)
            where TAggregate : IAggregateRoot;

        void RegisterDefaultStorage(IAggregateStorage storage);
    }
}