using EventFly.Core;

namespace EventFly.ReadModels
{
    public interface IAggregateReadModel<out TIdentity> : IReadModel<TIdentity>
    where TIdentity : IIdentity
    {
        long Version { get; }
    }
}