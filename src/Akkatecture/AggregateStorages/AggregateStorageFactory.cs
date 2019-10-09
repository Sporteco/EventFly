using System;
using System.Collections.Concurrent;
using Akkatecture.Aggregates;

namespace Akkatecture.AggregateStorages
{
    public class AggregateStorageFactory : IAggregateStorageFactory
    {
        private readonly ConcurrentDictionary<Type,Type> _items = new ConcurrentDictionary<Type, Type>();
        private Type _defaultStorage;
        public IAggregateStorage GetAggregateStorage<TAggregate>() where TAggregate : IAggregateRoot
        {
            if (!_items.ContainsKey(typeof(TAggregate))) return CreateStorage(_defaultStorage);
            return CreateStorage(_items[typeof(TAggregate)]);
        }

        private IAggregateStorage CreateStorage(Type storageType)
        {
            return (IAggregateStorage)Activator.CreateInstance(storageType);
        }

        public void RegisterStorage<TAggregate,TAggregateStorage>() 
            where TAggregate : IAggregateRoot
            where TAggregateStorage : IAggregateStorage
        {
            _items[typeof(TAggregate)] = typeof(TAggregateStorage);
        }

        public void RegisterDefaultStorage<TAggregateStorage>()
            where TAggregateStorage : IAggregateStorage
        {
            _defaultStorage = typeof(TAggregateStorage);
        }
    }
}