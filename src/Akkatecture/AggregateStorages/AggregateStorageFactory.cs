using System;
using System.Collections.Concurrent;
using Akkatecture.Aggregates;

namespace Akkatecture.AggregateStorages
{
    public class AggregateStorageFactory : IAggregateStorageFactory
    {
        private readonly ConcurrentDictionary<Type,IAggregateStorage> _items = new ConcurrentDictionary<Type, IAggregateStorage>();
        private IAggregateStorage _defaultStorage;
        public IAggregateStorage GetAggregateStorage<TAggregate>() where TAggregate : IAggregateRoot
        {
            if (!_items.ContainsKey(typeof(TAggregate))) return _defaultStorage;
            return _items[typeof(TAggregate)];
        }
        public void RegisterStorage<TAggregate>(IAggregateStorage storage) where TAggregate : IAggregateRoot
        {
            _items[typeof(TAggregate)] = storage;
        }

        public void RegisterDefaultStorage(IAggregateStorage storage)
        {
            _defaultStorage = storage;
        }
    }
}