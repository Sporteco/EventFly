using System;
using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Demo.Commands;
using Demo.Events;
using Demo.ValueObjects;

namespace Demo.Domain.Aggregates
{
    #region TestSagaId
    public class TestSagaId : Identity<TestSagaId> { public TestSagaId(string value) : base(value){} }
    
    #endregion
    
    public class TestSaga : StatelessSaga<TestSaga, TestSagaId>,
        ISagaIsStartedByAsync<UserId, UserCreatedEvent>,
        ISagaHandles<UserId,UserRenamedEvent>
    {
        private readonly string _s;

        public TestSaga(string s) : base()
        {
            _s = s;
        }

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
