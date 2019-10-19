using EventFly.Aggregates;
using EventFly.Core;

namespace EventFly.ReadModels
{
    public interface IAmReadModelFor<in TIdentity, in TAggregateEvent>
        where TIdentity : IIdentity
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
    {
        void Apply(IDomainEvent<TIdentity, TAggregateEvent> domainEvent);
    }
}
