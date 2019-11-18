using Demo.User.Commands;
using Demo.User.Events;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Sagas;
using EventFly.Sagas.AggregateSaga;
using System;
using System.Threading.Tasks;

namespace Demo.Application
{
    #region TestSagaId
    public class TestSagaId : Identity<TestSagaId> { public TestSagaId(String value) : base(value) { } }

    #endregion

    public class TestSaga : StatelessSaga<TestSaga, TestSagaId>,
        ISagaIsStartedBy<UserId, UserCreatedEvent>,
        ISagaHandles<UserId, UserRenamedEvent>
    {

        public async Task Handle(IDomainEvent<UserId, UserCreatedEvent> domainEvent)
        {
            await PublishCommandAsync(new RenameUserCommand(domainEvent.AggregateIdentity, new UserName(
                (DateTime.Now.ToLongDateString(), "ru"))));
        }

        public Task Handle(IDomainEvent<UserId, UserRenamedEvent> domainEvent)
        {
            Console.WriteLine($"FROM SAGA:Renamed to {domainEvent.AggregateEvent.NewName}");
            return Task.CompletedTask;
        }
    }
}
