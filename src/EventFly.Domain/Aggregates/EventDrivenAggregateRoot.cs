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
        public async override Task Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IEventMetadata metadata = null)
        {
            var applyMethods = GetEventApplyMethods(aggregateEvent);
            await applyMethods(aggregateEvent);

            await base.Emit(aggregateEvent, metadata);
        }

        protected EventDrivenAggregateRoot(TIdentity id) : base(id) { }

        protected Func<IAggregateEvent, Task> GetEventApplyMethods<TAggregateEvent>(TAggregateEvent aggregateEvent)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            var eventType = aggregateEvent.GetType();

            lock (_lockObject)
            {
                if (_applyMethodsFromState == null)
                {
                    _applyMethodsFromState = State.GetType().GetAggregateStateEventApplyMethods<TAggregate, TIdentity, TAggregateState>();
                }
            }
            if (!_applyMethodsFromState.TryGetValue(eventType, out var applyMethod))
            {
                var message = $"AggregateState of Type={State.GetType().PrettyPrint()} does not have an 'Apply' method that takes in an aggregate event of Type={eventType.PrettyPrint()} as an argument.";
                throw new NotImplementedException(message);
            }

            return applyMethod.Bind(State);
        }

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object _lockObject = new object();
        private static IReadOnlyDictionary<Type, Func<TAggregateState, IAggregateEvent, Task>> _applyMethodsFromState;
    }
}