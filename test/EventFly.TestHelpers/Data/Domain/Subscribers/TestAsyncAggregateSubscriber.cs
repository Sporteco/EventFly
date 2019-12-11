using System.Threading.Tasks;
using EventFly.Aggregates;
using EventFly.Subscribers;
using EventFly.Tests.Data.Abstractions;
using EventFly.Tests.Data.Abstractions.Events;

namespace EventFly.Tests.Data.Domain.Subscribers
{
    public class TestAsyncAggregateSubscriber : DomainEventSubscriber,
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
        public TAggregateEvent AggregateEvent { get; }

        public TestAsyncSubscribedEventHandled(TAggregateEvent aggregateEvent)
        {
            AggregateEvent = aggregateEvent;
        }
    }
}