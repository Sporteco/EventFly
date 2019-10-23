using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Exceptions;
using EventFly.Extensions;
using EventFly.Messages;

namespace EventFly.DomainService
{
    public class DomainServiceManager<TDomainService, TIdentity, TDomainServiceLocator> : ReceiveActor, IDomainServiceManager<TDomainService, TIdentity, TDomainServiceLocator>
        where TDomainService : ActorBase, IDomainService<TIdentity>
        where TIdentity : IIdentity
        where TDomainServiceLocator : class, IDomainServiceLocator<TIdentity>, new()
    {
        private IReadOnlyList<Type> _subscriptionTypes { get; }
        protected ILoggingAdapter Logger { get; }
        protected TDomainServiceLocator DomainServiceLocator { get; }
        public DomainServiceManagerSettings Settings { get; }

        public DomainServiceManager()
        {
            Logger = Context.GetLogger();

            _subscriptionTypes = new List<Type>();

            DomainServiceLocator = new TDomainServiceLocator();
            Settings = new DomainServiceManagerSettings(Context.System.Settings.Config);

            var DomainServiceType = typeof(TDomainService);

            if (Settings.AutoSubscribe)
            {
                var DomainServiceEventSubscriptionTypes =
                    DomainServiceType
                        .GetDomainServiceEventSubscriptionTypes();

                var asyncDomainServiceEventSubscriptionTypes =
                    DomainServiceType
                        .GetAsyncDomainEventSubscriberSubscriptionTypes();



                var subscriptionTypes = new List<Type>();
                
                subscriptionTypes.AddRange(DomainServiceEventSubscriptionTypes);
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
            var DomainServiceId = DomainServiceLocator.LocateService(domainEvent);
            var DomainService = FindOrSpawn(DomainServiceId);
            DomainService.Tell(domainEvent,Sender);
            return true;
        }

        protected virtual bool Terminate(Terminated message)
        {
            Logger.Warning("DomainService of Type={0}, and Id={1}; has terminated.",typeof(TDomainService).PrettyPrint(), message.ActorRef.Path.Name);
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

                    Logger.Warning("{0} will supervise Exception={1} to be decided as {2}.",GetType().PrettyPrint(), x.ToString(),Directive.Restart);
                    return Directive.Restart;
                });
        }

        protected IActorRef FindOrSpawn(TIdentity DomainServiceId)
        {
            var DomainService = Context.Child(DomainServiceId);
            if (DomainService.IsNobody())
            {
                return Spawn(DomainServiceId);
            }
            return DomainService;
        }

        private IActorRef Spawn(TIdentity DomainServiceId)
        {
            var DomainService = Context.ActorOf(Props.Create<TDomainService>(), DomainServiceId.Value);
            Context.Watch(DomainService);
            return DomainService;
        }
    }
}
