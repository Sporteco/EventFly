using System.Threading.Tasks;
using EventFly.Aggregates;
using EventFly.Core;
namespace EventFly.DomainService
{
    public interface IDomainServiceHandles<in TIdentity, in TAggregateEvent> 
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
        where TIdentity : IIdentity
    {
        bool Handle(IDomainEvent<TIdentity, TAggregateEvent> domainEvent);
    }

    public interface IDomainServiceHandlesAsync<in TIdentity, in TAggregateEvent>
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
        where TIdentity : IIdentity
    {
        Task HandleAsync(IDomainEvent<TIdentity, TAggregateEvent> domainEvent);
    }
}