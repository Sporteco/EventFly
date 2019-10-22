using System;
using EventFly.Aggregates;
using EventFly.ReadModels;
using Demo.Events;

namespace Demo.Infrastructure.ReadModels
{
    public class UsersInfoReadModel : ReadModel<UsersInfoReadModel>,
        IAmReadModelFor<UserId, UserCreatedEvent>,
        IAmReadModelFor<UserId, UserRenamedEvent>
    {
        public long Version { get; private set; }
        public string UserName { get; private set; }
        public DateTime Birth { get; private set; }

        public void Apply(IDomainEvent<UserId, UserCreatedEvent> domainEvent)
        {
            Version = domainEvent.Metadata.EventVersion;
            UserName = domainEvent.AggregateEvent.Name.Value;
            Birth = domainEvent.AggregateEvent.Birth.Value;
        }

        public void Apply(IDomainEvent<UserId, UserRenamedEvent> domainEvent)
        {
            Version = domainEvent.Metadata.EventVersion;
            UserName = domainEvent.AggregateEvent.NewName.Value;
        }
    }
}