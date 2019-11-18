using EventFly.Core;

namespace EventFly.ReadModels
{
    public interface IAggregateReadModel<out TIdentity>
    where TIdentity : IIdentity
    {
        System.Int64 Version { get; }
    }
}