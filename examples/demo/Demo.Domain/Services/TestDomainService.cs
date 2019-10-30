using Demo.Commands;
using Demo.Events;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Domain;
using System;
using System.Threading.Tasks;

namespace Demo.Domain.Services
{
    public class TestDomainService : DomainService<TestDomainService>,
        IDomainServiceIsStartedByAsync<UserId, UserCreatedEvent>,
        IDomainServiceHandles<UserId, UserRenamedEvent>

    {
        public Task HandleAsync(IDomainEvent<UserId, UserCreatedEvent> domainEvent)
        {
            return PublishCommandAsync(new RenameUserCommand(domainEvent.AggregateIdentity, new UserName("test")));
        }

        public Boolean Handle(IDomainEvent<UserId, UserRenamedEvent> domainEvent)
        {
            Console.Write("OK");
            return true;
        }
    }
}