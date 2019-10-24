using EventFly.Aggregates;
using EventFly.Core;

namespace EventFly.DomainService
{
    public interface IDomainServiceIsStartedBy<in TIdentity, in TAggregateEvent> : IDomainServiceHandles<TIdentity, TAggregateEvent>
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
        where TIdentity : IIdentity
    {
    }
}