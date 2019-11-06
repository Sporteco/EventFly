using EventFly.Core;
using EventFly.Exceptions;
using EventFly.Extensions;
using System;
using System.Collections.Generic;

namespace EventFly.Aggregates
{
    public abstract class EventDrivenAggregateRoot<TAggregate, TIdentity, TAggregateState> : AggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregate : EventDrivenAggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregateState : IAggregateState<TIdentity>
        where TIdentity : IIdentity
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object _lockObject = new object();
        private static IReadOnlyDictionary<Type, Action<TAggregateState, IAggregateEvent>> ApplyMethodsFromState;

        protected EventDrivenAggregateRoot(TIdentity id) : base(id)
        {
        }
        public override void Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IEventMetadata metadata = null)
        {
            var committedEvent = From(aggregateEvent, Version, metadata);

            var applyMethods = GetEventApplyMethods(committedEvent.AggregateEvent);
            applyMethods(committedEvent.AggregateEvent);

            ApplyCommittedEvent(committedEvent);
        }

        protected Action<IAggregateEvent> GetEventApplyMethods<TAggregateEvent>(TAggregateEvent aggregateEvent)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            var eventType = aggregateEvent.GetType();

            Action<TAggregateState, IAggregateEvent> applyMethod;
            lock (_lockObject)
            {
                if (ApplyMethodsFromState == null)
                {
                    ApplyMethodsFromState = State.GetType().GetAggregateStateEventApplyMethods<TAggregate, TIdentity, TAggregateState>();
                }
            }

            if (!ApplyMethodsFromState.TryGetValue(eventType, out applyMethod))
                throw new NotImplementedException($"AggregateState of Type={State.GetType().PrettyPrint()} does not have an 'Apply' method that takes in an aggregate event of Type={eventType.PrettyPrint()} as an argument.");

            var aggregateApplyMethod = applyMethod.Bind(State);

            return aggregateApplyMethod;
        }
    }
}