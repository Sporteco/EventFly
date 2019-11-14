using System;
using System.Threading.Tasks;
using EventFly.Core;
using EventFly.Exceptions;

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

        protected AggregateState()
        {
            if (!(this is TMessageApplier))
            {
                var message = $"MessageApplier of Type={GetType().PrettyPrint()} has a wrong generic argument Type={typeof(TMessageApplier).PrettyPrint()}.";
                throw new InvalidOperationException(message);
            }
        }
    }

    public abstract class AggregateState<TAggregate, TIdentity> : AggregateState<TAggregate, TIdentity, IMessageApplier<TAggregate, TIdentity>>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    { }
}