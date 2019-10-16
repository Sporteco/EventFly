using System.Collections.Concurrent;
using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.AggregateStorages
{
    public class InMemoryAggregateStorage<TAggregate> : IAggregateStorage<TAggregate>
        where TAggregate : IAggregateRoot
    {
        private readonly ConcurrentDictionary<string, object> _stages = new ConcurrentDictionary<string, object>();
        public void SaveState<TAggregateState, TIdentity>(TAggregateState state) where TAggregateState : IAggregateState<TIdentity> where TIdentity : IIdentity
        {
            _stages.AddOrUpdate(state.Id, state, (s, o) => state);
        }

        public TAggregateState LoadState<TAggregateState, TIdentity>(string id) where TAggregateState : class, IAggregateState<TIdentity>, new() where TIdentity : IIdentity
        {
            if (!_stages.ContainsKey(id)) return null;
            return (TAggregateState)_stages[id];
        }

        public void Dispose()
        {
            
        }
    }
}