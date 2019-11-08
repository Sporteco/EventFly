using Akka.Actor;
using EventFly.Core;
using EventFly.Exceptions;
using EventFly.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventFly.Aggregates
{
    public abstract class EventDrivenAggregateRoot<TAggregate, TIdentity, TAggregateState> : AggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregate : EventDrivenAggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregateState : IAggregateState<TIdentity>
        where TIdentity : IIdentity
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object _lockObject = new object();
        private static IReadOnlyDictionary<Type, Func<TAggregateState, IAggregateEvent, Task>> ApplyMethodsFromState;

        protected EventDrivenAggregateRoot(TIdentity id) : base(id)
        {
        }
        public async override Task Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IEventMetadata metadata = null)
        {
            var committedEvent = From(aggregateEvent, Version, metadata);

            var applyMethods = GetEventApplyMethods(committedEvent.AggregateEvent);
            await applyMethods(committedEvent.AggregateEvent);
            
            ApplyCommittedEvent(committedEvent);
        }

        protected Func<IAggregateEvent, Task> GetEventApplyMethods<TAggregateEvent>(TAggregateEvent aggregateEvent)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            var eventType = aggregateEvent.GetType();

            lock (_lockObject)
            {
                if (ApplyMethodsFromState == null)
                {
                    ApplyMethodsFromState = State.GetType().GetAggregateStateEventApplyMethods<TAggregate, TIdentity, TAggregateState>();
                }
            }

            if (!ApplyMethodsFromState.TryGetValue(eventType, out Func<TAggregateState, IAggregateEvent, Task> applyMethod))
                throw new NotImplementedException($"AggregateState of Type={State.GetType().PrettyPrint()} does not have an 'Apply' method that takes in an aggregate event of Type={eventType.PrettyPrint()} as an argument.");

            var aggregateApplyMethod = applyMethod.Bind(State);

            return aggregateApplyMethod;
        }
    }
}