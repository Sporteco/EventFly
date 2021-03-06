﻿using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using EventFly.Aggregates;
using EventFly.Extensions;
using System;

namespace EventFly.ReadModels
{
    public abstract class ReadModelManager<TReadModel> : ReceiveActor, IReadModelManager
        where TReadModel : ReadModel, new()
    {
        protected ILoggingAdapter Logger { get; set; }

        public String Name { get; }

        public ReadModelManager()
        {
            Logger = Context.GetLogger();
            Name = GetType().PrettyPrint();

            Receive<Terminated>(Terminate);

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            var subscriptionTypes =
                typeof(TReadModel).GetReadModelSubscribersTypes();

            foreach (var subscriptionType in subscriptionTypes)
            {
                var domainEventType = typeof(IDomainEvent<,>).MakeGenericType(subscriptionType.Item1, subscriptionType.Item2);

                Context.System.EventStream.Subscribe(Self, domainEventType);
                Receive(domainEventType, e => Dispatch((IDomainEvent)e));
            }
        }

        protected virtual Boolean Dispatch(IDomainEvent domainEvent)
        {
            Logger.Info("ReadModelManager of Type={0}; has received a domain event of Type={1}", Name, typeof(TReadModel).PrettyPrint());

            var readModelId = GetReadModelId(domainEvent);
            var aggregateRef = FindOrCreate(readModelId);

            aggregateRef.Forward(domainEvent);

            return true;
        }

        protected abstract String GetReadModelId(IDomainEvent domainEvent);


        protected virtual Boolean ReDispatch(IDomainEvent domainEvent)
        {
            Logger.Info("ReadModelManager of Type={0}; is ReDispatching deadletter of Type={1}", Name, typeof(TReadModel).PrettyPrint());

            var readModelId = GetReadModelId(domainEvent);
            var aggregateRef = FindOrCreate(readModelId);

            aggregateRef.Forward(domainEvent);

            return true;
        }

        protected virtual Boolean Terminate(Terminated message)
        {
            Logger.Warning("Query of Type={0}, and Id={1}; has terminated.", typeof(TReadModel).PrettyPrint(), message.ActorRef.Path.Name);
            Context.Unwatch(message.ActorRef);
            return true;
        }

        protected virtual IActorRef FindOrCreate(String readModelId)
        {
            var aggregate = Context.Child(readModelId);

            if (aggregate.IsNobody())
            {
                aggregate = CreateReadModelHandler(readModelId);
            }

            return aggregate;
        }

        protected virtual IActorRef CreateReadModelHandler(String readModelId)
        {
            Props props;
            try
            {
                props = Context.DI().Props<ReadModelHandler<TReadModel>>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "No DI available at the moment, falling back to default props creation.");
                props = Props.Create<ReadModelHandler<TReadModel>>();
            }

            var aggregateRef = Context.ActorOf(props, readModelId);
            Context.Watch(aggregateRef);
            return aggregateRef;
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            var logger = Logger;
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeMilliseconds: 3000,
                localOnlyDecider: x =>
                {

                    logger.Warning("ReadModelManager of Type={0}; will supervise Exception={1} to be decided as {2}.", Name, x.ToString(), Directive.Restart);
                    return Directive.Restart;
                });
        }

    }
}