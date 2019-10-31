using System;
using System.Threading.Tasks;
using EventFly.Aggregates;
using EventFly.Core;
using Demo.ValueObjects;
using EventFly.Sagas;
using EventFly.Sagas.AggregateSaga;
using Demo.User;

namespace Demo.Application
{
    #region TestSagaId
    public class TestSagaId : Identity<TestSagaId> { public TestSagaId(string value) : base(value){} }
    
    #endregion
    
    public class TestSaga : StatelessSaga<TestSaga, TestSagaId>,
        ISagaIsStartedByAsync<UserId, UserCreatedEvent>,
        ISagaHandles<UserId,UserRenamedEvent>
    {

        public async Task HandleAsync(IDomainEvent<UserId, UserCreatedEvent> domainEvent)
        {
            await PublishCommandAsync(new RenameUserCommand(domainEvent.AggregateIdentity, new UserName(
                (DateTime.Now.ToLongDateString(), "ru"))));
        }

        public bool Handle(IDomainEvent<UserId, UserRenamedEvent> domainEvent)
        {
            Console.WriteLine($"FROM SAGA:Renamed to {domainEvent.AggregateEvent.NewName}");
            return true;
        }
    }
}
