using System;
using EventFly.Aggregates;
using EventFly.ReadModels;
using Demo.Events;

namespace Demo.Domain.ReadModels
{
    public class UsersInfoReadModel : ReadModel<UserId>,
        IAmReadModelFor<UserId, UserCreatedEvent>,
        IAmReadModelFor<UserId, UserRenamedEvent>
    {
        public long Version { get; private set; }
        public string UserName { get; private set; }
        public DateTime Birth { get; private set; }

        public void Apply(IReadModelContext context, IDomainEvent<UserId, UserCreatedEvent> domainEvent)
        {
            Id = domainEvent.AggregateIdentity;
            Version = domainEvent.Metadata.EventVersion;
            UserName = domainEvent.AggregateEvent.Name.Value;
            Birth = domainEvent.AggregateEvent.Birth.Value;
        }

        public void Apply(IReadModelContext context,IDomainEvent<UserId, UserRenamedEvent> domainEvent)
        {
            Version = domainEvent.Metadata.EventVersion;
            UserName = domainEvent.AggregateEvent.NewName.Value;
        }
    }
}