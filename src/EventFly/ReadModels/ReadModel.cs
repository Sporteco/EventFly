using System;
using Akka.Actor;
using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Extensions;

namespace EventFly.ReadModels
{
    public abstract class ReadModel<TKey> : ReceiveActor, IReadModel<TKey>
    {
        protected ReadModel()
        {
            this.InitReadModelReceivers(Context.System.EventStream, Context.Parent);

            
        }
        protected void Command<TIdentity, TAggregateEvent>(Action<IReadModelContext, IDomainEvent<TIdentity, TAggregateEvent>> handler)
            where TIdentity : IIdentity
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            Receive<IDomainEvent<TIdentity, TAggregateEvent>>(e =>
            {
                var context = new ReadModelContext(Self.Path.Name); 
                handler(context, e);
            });
        }

        public TKey Id { get; protected set; }
    }
}