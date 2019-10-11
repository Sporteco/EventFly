using Akkatecture.Core;

namespace Akkatecture.ReadModels
{
    public interface IAggregateReadModel<out TIdentity> : IReadModel<TIdentity>
    where TIdentity : IIdentity
    {
        long Version { get; }
    }
}