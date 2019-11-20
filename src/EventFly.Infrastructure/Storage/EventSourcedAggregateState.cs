using EventFly.Aggregates;
using EventFly.Core;
using System.Threading.Tasks;

namespace EventFly.Infrastructure.Storage
{
    public class EventSourcedAggregateState<TAggregateState, TIdentity> : AggregateState<TAggregateState, TIdentity>
        where TAggregateState : class, IAggregateState<TIdentity>, IMessageApplier<TIdentity>
        where TIdentity : IIdentity
    {
        public EventSourcedAggregateState(IAggregateStateEventStore db)
        {
            Db = db;
        }

        public override async Task LoadState(TIdentity id)
        {
            Id = id;
            var events = await Db.GetEvents(Id);
            foreach (var @event in events) Apply(@event);
        }

        protected override async Task PostApplyAction(IAggregateEvent<TIdentity> @event)
        {
            await Db.Put(Id, @event);
        }

        protected readonly IAggregateStateEventStore Db;
    }
}