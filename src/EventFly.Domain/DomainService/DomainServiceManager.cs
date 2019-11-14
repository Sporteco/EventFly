using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using EventFly.Aggregates;
using EventFly.Exceptions;
using EventFly.Extensions;
using EventFly.Messages;

namespace EventFly.DomainService
{
    public class DomainServiceManager<TDomainService> : ReceiveActor, IDomainServiceManager<TDomainService>
        where TDomainService : ActorBase, IDomainService
    {
        public DomainServiceManagerSettings Settings { get; }

        public DomainServiceManager()
        {
            Logger = Context.GetLogger();

            _subscriptionTypes = new List<Type>();

            Settings = new DomainServiceManagerSettings(Context.System.Settings.Config);

            var domainServiceType = typeof(TDomainService);

            if (Settings.AutoSubscribe)
            {
                var types = domainServiceType.GetDomainServiceEventSubscriptionTypes();
                var asyncTypes = domainServiceType.GetAsyncDomainServiceEventSubscriptionTypes();
                _subscriptionTypes = types.Concat(asyncTypes).ToList().AsReadOnly();
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
            var service = FindOrSpawn();
            service.Tell(domainEvent, Sender);
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

        protected IActorRef FindOrSpawn()
        {
            var service = Context.Child(GetDomainServiceId());
            return service.IsNobody() ? Spawn() : service;
        }

        private readonly IReadOnlyList<Type> _subscriptionTypes;

        private IActorRef Spawn()
        {
            var service = Context.ActorOf(Props.Create<TDomainService>(), GetDomainServiceId());
            Context.Watch(service);
            return service;
        }

        private static string GetDomainServiceId() =>  typeof(TDomainService).FullName;
    }
}