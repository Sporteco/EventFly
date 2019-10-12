using System;
using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Demo.Commands;
using Demo.Events;
using Demo.ValueObjects;

namespace Demo.Domain
{
    public class TestSagaId : Identity<TestSagaId>
    {
        public TestSagaId(string value) : base(value){}
    }

    public class TestSaga : StatelessSaga<TestSaga, TestSagaId>,
        ISagaIsStartedByAsync<UserId, UserCreatedEvent>,
        ISagaHandles<UserId,UserRenamedEvent>
    {

        public async Task HandleAsync(IDomainEvent<UserId, UserCreatedEvent> domainEvent)
        {
            await PublishCommandAsync(new RenameUserCommand(domainEvent.AggregateIdentity, new UserName(DateTime.Now.ToLongDateString())));
        }

        public bool Handle(IDomainEvent<UserId, UserRenamedEvent> domainEvent)
        {
            Console.WriteLine($"FROM SAGA:Renamed to {domainEvent.AggregateEvent.NewName}");
            return true;
        }
    }
}
