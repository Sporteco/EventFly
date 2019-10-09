using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.AggregateStorages
{
    public interface IAggregateStorage
    {
        void SaveState<TAggregateState, TIdentity>(TAggregateState state)
            where TAggregateState : IAggregateState<TIdentity>
            where TIdentity : IIdentity;
        TAggregateState LoadState<TAggregateState, TIdentity>(TIdentity id)
            where TAggregateState : class, IAggregateState<TIdentity>
            where TIdentity : IIdentity;

    }
}
