using EventFly.Core;
using System.Threading.Tasks;

namespace EventFly.Aggregates
{
    public abstract class EventDrivenAggregateRoot<TAggregate, TIdentity, TAggregateState> : AggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregate : EventDrivenAggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregateState : IAggregateState<TIdentity>
        where TIdentity : IIdentity
    {
        public async override Task Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IEventMetadata metadata = null)
        {
            await State.UpdateAndSaveState(aggregateEvent);
            await base.Emit(aggregateEvent, metadata);
        }

        protected EventDrivenAggregateRoot(TIdentity id) : base(id) { }
    }
}