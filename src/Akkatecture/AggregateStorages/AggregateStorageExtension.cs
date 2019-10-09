using Akka.Actor;
using Akkatecture.Aggregates;

namespace Akkatecture.AggregateStorages
{
    public class AggregateStorageExtension : IExtension
    {
        private IAggregateStorageFactory _storageFactory;
    
        public void Initialize(IAggregateStorageFactory storageFactory)
        {
            _storageFactory = storageFactory;
        }
    
        public IAggregateStorage GetAggregateStorage<TAggregate>()
            where TAggregate : IAggregateRoot
        {
            return _storageFactory.GetAggregateStorage<TAggregate>();
        }
    }
}