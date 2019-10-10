using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.ReadModels
{
    public interface IAmReadModelFor<in TIdentity, in TAggregateEvent>
        where TIdentity : IIdentity
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
    {
        void Apply(IReadModelContext context, IDomainEvent<TIdentity, TAggregateEvent> domainEvent);
    }
}
