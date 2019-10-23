using System;
using EventFly.Core;

namespace EventFly.Domain.Aggregates
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
