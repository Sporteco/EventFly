using EventFly.Aggregates.Snapshot;
using EventFly.Core;

namespace EventFly.Aggregates
{
    public interface IMessageApplier<TAggregate, TIdentity> : IEventApplier<TAggregate, TIdentity>, ISnapshotHydrater<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        
    }
}