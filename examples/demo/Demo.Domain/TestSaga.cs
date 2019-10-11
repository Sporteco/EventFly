using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Demo.Events;

namespace Demo.Domain
{
    public class TestSagaId : SagaId<TestSagaId>
    {
        public TestSagaId(string value) : base(value){}
    }
    public class TestSagaState : SagaState<TestSaga,TestSagaId,IMessageApplier<TestSaga,TestSagaId>>{
    }
    public class TestSaga : AggregateSaga<TestSaga, TestSagaId, TestSagaState>,
        ISagaIsStartedByAsync<UserId, UserCreatedEvent>
    {
        public Task HandleAsync(IDomainEvent<UserId, UserCreatedEvent> domainEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}
