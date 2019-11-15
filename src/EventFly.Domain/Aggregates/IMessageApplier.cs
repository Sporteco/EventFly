using EventFly.Aggregates.Snapshot;
using EventFly.Core;

namespace EventFly.Aggregates
{
    public interface IMessageApplier<TIdentity> : IEventApplier<TIdentity>, ISnapshotHydrater<TIdentity>
        where TIdentity : IIdentity
    {
        
    }
}