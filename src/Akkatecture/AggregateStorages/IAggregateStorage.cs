using System;
using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.AggregateStorages
{
    public interface IAggregateStorage : IDisposable
    {
        void SaveState<TAggregateState, TIdentity>(TAggregateState state)
            where TAggregateState : IAggregateState<TIdentity>
            where TIdentity : IIdentity;
        TAggregateState LoadState<TAggregateState, TIdentity>(TIdentity id)
            where TAggregateState : class, IAggregateState<TIdentity>,new()
            where TIdentity : IIdentity;

    }
}
