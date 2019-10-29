using EventFly.Aggregates;
using EventFly.Core;
using System;
using System.Threading.Tasks;

namespace EventFly.Domain
{
    public interface IDomainServiceHandles<in TIdentity, in TAggregateEvent>
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
        where TIdentity : IIdentity
    {
        Boolean Handle(IDomainEvent<TIdentity, TAggregateEvent> domainEvent);
    }

    public interface IDomainServiceHandlesAsync<in TIdentity, in TAggregateEvent>
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
        where TIdentity : IIdentity
    {
        Task HandleAsync(IDomainEvent<TIdentity, TAggregateEvent> domainEvent);
    }
}