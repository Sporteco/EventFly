using Akka.Actor;
using Akka.Event;
using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Exceptions;
using EventFly.Extensions;
using EventFly.Messages;
using System;
using System.Collections.Generic;

namespace EventFly.Domain
{
    public class DomainServiceManager<TDomainService, TIdentity, TDomainServiceLocator> : ReceiveActor, IDomainServiceManager<TDomainService, TIdentity, TDomainServiceLocator>
        where TDomainService : ActorBase, IDomainService<TIdentity>
        where TIdentity : IIdentity
        where TDomainServiceLocator : class, IDomainServiceLocator<TIdentity>, new()
    {
        public DomainServiceManagerSettings Settings { get; }

        public DomainServiceManager()
        {
            Logger = Context.GetLogger();

            _subscriptionTypes = new List<Type>();

            DomainServiceLocator = new TDomainServiceLocator();
            Settings = new DomainServiceManagerSettings(Context.System.Settings.Config);

            var domainServiceType = typeof(TDomainService);

            if (Settings.AutoSubscribe)
            {
                var domainServiceEventSubscriptionTypes = domainServiceType.GetDomainServiceEventSubscriptionTypes();
                var asyncDomainServiceEventSubscriptionTypes = domainServiceType.GetAsyncDomainEventSubscriberSubscriptionTypes();
                var subscriptionTypes = new List<Type>();

                subscriptionTypes.AddRange(domainServiceEventSubscriptionTypes);
                subscriptionTypes.AddRange(asyncDomainServiceEventSubscriptionTypes);

                _subscriptionTypes = subscriptionTypes.AsReadOnly();
                foreach (var type in _subscriptionTypes)
                {
                    Context.System.EventStream.Subscribe(Self, type);
                }
            }

            if (Settings.AutoSpawnOnReceive)
            {
                Receive<IDomainEvent>(Handle);
            }

            Receive<UnsubscribeFromAll>(Handle);
        }

        protected ILoggingAdapter Logger { get; }
        protected TDomainServiceLocator DomainServiceLocator { get; }

        protected virtual bool Handle(UnsubscribeFromAll command)
        {
            UnsubscribeFromAllTopics();

            return true;
        }

        protected void UnsubscribeFromAllTopics()
        {
            foreach (var type in _subscriptionTypes)
            {
                Context.System.EventStream.Unsubscribe(Self, type);
            }
        }

        protected virtual bool Handle(IDomainEvent domainEvent)
        {
            var domainServiceId = DomainServiceLocator.LocateDomainService(domainEvent);
            var domainService = FindOrSpawn(domainServiceId);
            domainService.Tell(domainEvent, Sender);
            return true;
        }

        protected virtual bool Terminate(Terminated message)
        {
            Logger.Warning("DomainService of Type={0}, and Id={1}; has terminated.", typeof(TDomainService).PrettyPrint(), message.ActorRef.Path.Name);
            Context.Unwatch(message.ActorRef);
            return true;
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeMilliseconds: 3000,
                localOnlyDecider: x =>
                {
                    Logger.Warning("{0} will supervise Exception={1} to be decided as {2}.", GetType().PrettyPrint(), x.ToString(), Directive.Restart);
                    return Directive.Restart;
                });
        }

        protected IActorRef FindOrSpawn(TIdentity domainServiceId)
        {
            var domainService = Context.Child(domainServiceId);
            if (domainService.IsNobody())
            {
                return Spawn(domainServiceId);
            }
            return domainService;
        }

        private readonly IReadOnlyList<Type> _subscriptionTypes;

        private IActorRef Spawn(TIdentity domainServiceId)
        {
            var domainService = Context.ActorOf(Props.Create<TDomainService>(), domainServiceId.Value);
            Context.Watch(domainService);
            return domainService;
        }
    }
}