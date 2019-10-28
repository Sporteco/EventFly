using System;
using System.Threading.Tasks;
using Demo.Commands;
using Demo.Events;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Core;
using EventFly.DomainService;


namespace Demo.Domain.Services
{
    public class TestDomainServiceId : Identity<TestDomainServiceId>
    {
        public TestDomainServiceId(string value) : base(value)
        {
        }
    }
    public class TestDomainService : DomainService<TestDomainService,TestDomainServiceId>,
        IDomainServiceIsStartedByAsync<UserId, UserCreatedEvent>,
        IDomainServiceHandles<UserId, UserRenamedEvent>

    {
        public Task HandleAsync(IDomainEvent<UserId, UserCreatedEvent> domainEvent)
        {
            return PublishCommandAsync(new RenameUserCommand(domainEvent.AggregateIdentity, new UserName(("test", "ru"))));
        }

        public bool Handle(IDomainEvent<UserId, UserRenamedEvent> domainEvent)
        {
            Console.Write("OK");
            return true;
        }
    }
}
