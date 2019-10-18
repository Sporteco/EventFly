using System.Threading.Tasks;
using EventFly.Aggregates;
using EventFly.Subscribers;
using EventFly.TestHelpers.Aggregates;
using EventFly.TestHelpers.Aggregates.Events;

namespace EventFly.TestHelpers.Subscribers
{
    public class TestAsyncAggregateSubscriber: DomainEventSubscriber,
        ISubscribeToAsync<TestAggregateId, TestCreatedEvent>,
        ISubscribeToAsync<TestAggregateId, TestAddedEvent>
    {
        public Task HandleAsync(IDomainEvent<TestAggregateId, TestCreatedEvent> domainEvent)
        {
            var handled = new TestAsyncSubscribedEventHandled<TestCreatedEvent>(domainEvent.AggregateEvent);
            Context.System.EventStream.Publish(handled);
            return Task.CompletedTask;
        }
        
        public Task HandleAsync(IDomainEvent<TestAggregateId, TestAddedEvent> domainEvent)
        {
            var handled = new TestAsyncSubscribedEventHandled<TestAddedEvent>(domainEvent.AggregateEvent);
            Context.System.EventStream.Publish(handled);
            
            return Task.CompletedTask;
        }
    }

    public class TestAsyncSubscribedEventHandled<TAggregateEvent> 
    {
        public TAggregateEvent AggregateEvent { get;}

        public TestAsyncSubscribedEventHandled(TAggregateEvent aggregateEvent)
        {
            AggregateEvent = aggregateEvent;
        }
    }
}