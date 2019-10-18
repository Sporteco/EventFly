using System;
using EventFly.Aggregates;
using EventFly.Core;

namespace EventFly.AggregateStorages
{
    public interface IAggregateStorage<TAggregate> : IDisposable
        where TAggregate : IAggregateRoot
    {
        void SaveState<TAggregateState, TIdentity>(TAggregateState state)
            where TAggregateState : IAggregateState<TIdentity>
            where TIdentity : IIdentity;
        TAggregateState LoadState<TAggregateState, TIdentity>(string id)
            where TAggregateState : class, IAggregateState<TIdentity>, new()
            where TIdentity : IIdentity;

    }
}
