using EventFly.Core;
using EventFly.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventFly.Extensions;

namespace EventFly.Aggregates
{
    public abstract class AggregateState<TAggregateState, TIdentity, TMessageApplier> : IAggregateState<TIdentity>, IMessageApplier<TIdentity>
        where TAggregateState : IAggregateState<TIdentity>, IMessageApplier< TIdentity>
        where TMessageApplier : class, IMessageApplier< TIdentity>
        where TIdentity : IIdentity
    {
        private static readonly IReadOnlyDictionary<Type, Action<TMessageApplier, IAggregateEvent>> ApplyMethods; 

        static AggregateState()
        {
            ApplyMethods = typeof(TMessageApplier).GetAggregateEventApplyMethods<TIdentity, TMessageApplier>();
        }
        public TIdentity Id { get; set; }

        public virtual Task LoadState(TIdentity id)
        {
            Id = id;
            return Task.CompletedTask;
        }

        public async Task UpdateAndSaveState(IAggregateEvent<TIdentity> @event)
        {
            var eventType = @event.GetType();
            var applyMethod = GetApplierOfConcreteEvent(eventType);
            if (applyMethod == null)
            {
                var message = $"'{GetType().PrettyPrint()}' does not have an 'Apply' method that takes in a '{eventType.PrettyPrint()}'.";
                throw new NotImplementedException(message);
            }

            await PreApplyAction(@event);
            applyMethod.Invoke((TMessageApplier) (object) this, @event );
            await PostApplyAction(@event);
        }

        protected AggregateState()
        {
            if (!(this is TMessageApplier))
            {
                var message = $"MessageApplier of Type={GetType().PrettyPrint()} has a wrong generic argument Type={typeof(TMessageApplier).PrettyPrint()}.";
                throw new InvalidOperationException(message);
            }
        }

        protected void Apply(IAggregateEvent<TIdentity> @event)
        {
            var applier = GetApplierOfConcreteEvent(@event.GetType());
            applier((TMessageApplier)(object)this,  @event );
        }

        protected virtual Task PreApplyAction(IAggregateEvent<TIdentity> @event) => Task.CompletedTask;
        protected virtual Task PostApplyAction(IAggregateEvent<TIdentity> @event) => Task.CompletedTask;

        private  Action<TMessageApplier, IAggregateEvent> GetApplierOfConcreteEvent(Type eventType) 
            => !ApplyMethods.TryGetValue(eventType, out var applier) ? null : applier;
    }

    public abstract class AggregateState<TAggregateState, TIdentity> : AggregateState<TAggregateState,  TIdentity, TAggregateState>
        where TIdentity : IIdentity
        where TAggregateState : class, IAggregateState<TIdentity>, IMessageApplier<TIdentity>
    { }
}
