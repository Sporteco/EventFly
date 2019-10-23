using EventFly.Core;
using EventFly.Domain.Aggregates.Snapshot;

namespace EventFly.Domain.Aggregates
{
    public interface IMessageApplier<TAggregate, TIdentity> : IEventApplier<TAggregate, TIdentity>, ISnapshotHydrater<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {

    }
}