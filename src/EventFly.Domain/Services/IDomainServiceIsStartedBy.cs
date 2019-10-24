using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Sagas;

namespace EventFly.Domain.Services
{
    public interface IDomainServiceIsStartedBy<in TIdentity, in TAggregateEvent> : ISagaHandles<TIdentity, TAggregateEvent>
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
        where TIdentity : IIdentity
    {
    }
}