using System.Collections.Concurrent;
using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.AggregateStorages
{
    public class InMemoryAggregateStorage : IAggregateStorage
    {
        private readonly ConcurrentDictionary<string, object> _stages = new ConcurrentDictionary<string, object>();
        public void SaveState<TAggregateState, TIdentity>(TAggregateState state) where TAggregateState : IAggregateState<TIdentity> where TIdentity : IIdentity
        {
            _stages.AddOrUpdate(state.Id.Value, state, (s, o) => state);
        }

        public TAggregateState LoadState<TAggregateState, TIdentity>(TIdentity id) where TAggregateState : class, IAggregateState<TIdentity>, new() where TIdentity : IIdentity
        {
            if (!_stages.ContainsKey(id.Value)) return null;
            return (TAggregateState)_stages[id.Value];
        }

        public void Dispose()
        {
            
        }
    }
}