using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using Akka.Event;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Events;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates
{
     public abstract class AggregateRoot<TAggregate, TIdentity, TAggregateState> : ReceiveActor, IAggregateRoot<TIdentity>
        where TAggregate : AggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregateState : class, IAggregateState<TIdentity>, new ()
        where TIdentity : IIdentity
    {
        private readonly IEventDefinitionService _eventDefinitionService;

        private static readonly IAggregateName AggregateName = typeof(TAggregate).GetAggregateName();
        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(100);
        private ICommand<TIdentity> PinnedCommand { get; set; }
        private object PinnedReply { get; set; }
        public TAggregateState State { get; private set; }
        public IAggregateName Name => AggregateName;
        public long Version => 1;
        public bool IsNew => false;
        public TIdentity Id { get; }

        protected readonly ILoggingAdapter Log = Context.GetLogger();
        protected AggregateRoot(TIdentity id)
        {
            
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            
            if ((this as TAggregate) == null)
            {
                throw new InvalidOperationException(
                    $"Aggregate {Name} specifies Type={typeof(TAggregate).PrettyPrint()} as generic argument, it should be its own type.");
            }

            if (State == null)
            {
                try
                {
                    State = (TAggregateState)Activator.CreateInstance(typeof(TAggregateState));
                }
                catch(Exception exception)
                {
                    Context.GetLogger().Error(exception,"Unable to activate AggregateState of Type={0} for AggregateRoot of Name={1}.",typeof(TAggregateState).PrettyPrint(), Name);
                }

            }

            PinnedCommand = null;
            _eventDefinitionService = new EventDefinitionService(Context.GetLogger());
            Id = id;
            SetSourceIdHistory(100);

            InitReceives();
        }

        protected override void PreStart()
        {
            base.PreStart();
            State = LoadState();
        }

        protected abstract TAggregateState LoadState();

        protected abstract void SaveState();

        public void InitReceives()
        {
            var type = GetType();
            
            var subscriptionTypes =
                type.GetAggregateExecuteTypes();

            var methods = type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Execute") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1;
                })
                .ToDictionary(
                    mi => mi.GetParameters()[0].ParameterType,
                    mi => mi);
            
            var method = type
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Command") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1
                        && parameters[0].ParameterType.Name.Contains("Func");
                })
                .First();

            foreach (var subscriptionType in subscriptionTypes)
            {
                var funcType = typeof(Func<,>).MakeGenericType(subscriptionType, typeof(bool));
                var subscriptionFunction = Delegate.CreateDelegate(funcType, this, methods[subscriptionType]);
                var actorReceiveMethod = method.MakeGenericMethod(subscriptionType);

                actorReceiveMethod.Invoke(this, new object[] { subscriptionFunction });
            }
        }
        protected void Command<T>(Func<T,bool> handler)
        {
            Receive(handler);
        }
        protected void SetSourceIdHistory(int count)
        {
            _previousSourceIds = new CircularBuffer<ISourceId>(count);
        }

        public bool HasSourceId(ISourceId sourceId)
        {
            return !sourceId.IsNone() && _previousSourceIds.Any(s => s.Value == sourceId.Value);
        }

        public IIdentity GetIdentity()
        {
            return Id;
        }

        public virtual void Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IMetadata metadata = null)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            var committedEvent = From(aggregateEvent, Version, metadata);
            SaveState();
            ApplyCommittedEvent(committedEvent);
        }

        public virtual CommittedEvent<TAggregate, TIdentity, TAggregateEvent> From<TAggregateEvent>(TAggregateEvent aggregateEvent,
            long version, IMetadata metadata = null)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }
            _eventDefinitionService.Load(aggregateEvent.GetType());
            var eventDefinition = _eventDefinitionService.GetDefinition(aggregateEvent.GetType());
            var aggregateSequenceNumber = version + 1;
            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{Id.Value}-v{aggregateSequenceNumber}");
            var now = DateTimeOffset.UtcNow;
            var eventMetadata = new Metadata
            {
                Timestamp = now,
                AggregateSequenceNumber = aggregateSequenceNumber,
                AggregateName = Name.Value,
                AggregateId = Id.Value,
                SourceId = PinnedCommand.SourceId,
                EventId = eventId,
                EventName = eventDefinition.Name,
                EventVersion = eventDefinition.Version
            };
            eventMetadata.Add(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }
            
            var committedEvent = new CommittedEvent<TAggregate, TIdentity, TAggregateEvent>(Id, aggregateEvent,eventMetadata,now,aggregateSequenceNumber);
            return committedEvent;
        }

        protected void  ApplyCommittedEvent<TAggregateEvent>(ICommittedEvent<TAggregate, TIdentity, TAggregateEvent> committedEvent)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            Log.Info("Aggregate of Name={0}, and Id={1}; committed and applied an AggregateEvent of Type={2}.", Name, Id, typeof(TAggregateEvent).PrettyPrint());

            var domainEvent = new DomainEvent<TAggregate,TIdentity,TAggregateEvent>(Id, committedEvent.AggregateEvent,committedEvent.Metadata,committedEvent.Timestamp,Version);

            Publish(domainEvent);
            ReplyIfAvailable();
            
        }


        protected virtual void Publish<TEvent>(TEvent aggregateEvent)
        {
            Context.System.EventStream.Publish(aggregateEvent);
            Log.Info("Aggregate of Name={0}, and Id={1}; published DomainEvent of Type={2}.",Name, Id, typeof(TEvent).PrettyPrint());
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            if (message is Command<TIdentity> command)
            {
                if(IsNew || Id.Equals(command.AggregateId))
                    PinnedCommand = command;
            }

            var result = base.AroundReceive(receive, message);
            //Сохраняем состояние после каждой команды 
            SaveState();
            return result;
        }

        protected virtual void Reply(object replyMessage)
        {
            if(!Sender.IsNobody())
            {
                PinnedReply = replyMessage;
            }
        }
        
        protected virtual void ReplyFailure(object replyMessage)
        {
            if(!Sender.IsNobody())
            {
                Context.Sender.Tell(replyMessage);
            }
        }

        protected virtual void ReplyIfAvailable()
        {
            if(PinnedReply != null)
                Sender.Tell(PinnedReply);

            PinnedReply = null;
            PinnedCommand = null;
        }

        protected override void Unhandled(object message)
        {
            Log.Warning("Aggregate of Name={0}, and Id={1}; has received an unhandled message of Type={2}.",Name, Id, message.GetType().PrettyPrint());
            base.Unhandled(message);
        }

        protected IEnumerable<IAggregateEvent<TIdentity>> Events(params IAggregateEvent<TIdentity>[] events)
        {
            return events.ToList();
        }

        public override string ToString()
        {
            return $"{GetType().PrettyPrint()} v{Version}";
        }

        public bool Timeout(ReceiveTimeout message)
        {
            Log.Debug("Aggregate of Name={0}, and Id={1}; has received a timeout message and will stop.", Name, Id);
            Context.Stop(Self);
            return true;
        }
        
        public override void AroundPreRestart(Exception cause, object message)
        {
            Log.Error(cause, "Aggregate of Name={0}, and Id={1}; has experienced an error and will now restart", Name, Id);
            base.AroundPreRestart(cause, message);
        }
    }
}
