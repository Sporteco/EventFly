using EventFly.Aggregates;
using EventFly.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFly.Infrastructure.Storage
{
    public class InMemoryAggregateStateEventStore : IAggregateStateEventStore
    {
        public InMemoryAggregateStateEventStore()
        {
            _db = new ConcurrentDictionary<IIdentity, HashSet<IAggregateEvent>>();
        }

        public Task<IAggregateEvent<T>[]> GetEvents<T>(T id)
            where T : IIdentity
        {
            if (!_db.TryGetValue(id, out var result)) result = new HashSet<IAggregateEvent>();
            return Task.FromResult(result.Cast<IAggregateEvent<T>>().ToArray());
        }

        public Task Put<T>(T id, IAggregateEvent<T> @event)
            where T : IIdentity
        {
            var events = _db.GetOrAdd(id, new HashSet<IAggregateEvent>());
            events.Add(@event);
            return Task.CompletedTask;
        }

        private readonly ConcurrentDictionary<IIdentity, HashSet<IAggregateEvent>> _db;
    }
}