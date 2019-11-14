using EventFly.Core;
using EventFly.Exceptions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventFly.Aggregates
{
    public abstract class AggregateState<TAggregate, TIdentity, TMessageApplier> : IAggregateState<TIdentity>, IMessageApplier<TAggregate, TIdentity>
        where TMessageApplier : class, IMessageApplier<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
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
            applyMethod.Invoke(this, new[] { @event });
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
            applier?.Invoke(this, new Object[] { @event });
        }

        protected virtual Task PreApplyAction(IAggregateEvent<TIdentity> @event) => Task.CompletedTask;
        protected virtual Task PostApplyAction(IAggregateEvent<TIdentity> @event) => Task.CompletedTask;

        private MethodInfo GetApplierOfConcreteEvent(Type eventType)
        {
            foreach (var method in GetType().GetRuntimeMethods().Where(x => x.Name == "Apply"))
            {
                var parameters = method.GetParameters();
                if (parameters.Length != 1) continue;
                if (parameters[0].ParameterType == eventType) return method;
            }
            return null;
        }
    }

    public abstract class AggregateState<TAggregate, TIdentity> : AggregateState<TAggregate, TIdentity, IMessageApplier<TAggregate, TIdentity>>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    { }
}
