// The MIT License (MIT)
//
// Copyright (c) 2015-2019 Rasmus Mikkelsen
// Copyright (c) 2015-2019 eBay Software Foundation
// Modified from original source https://github.com/eventflow/EventFlow
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/EventFly
//
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using EventFly.Aggregates.Snapshot;
using EventFly.Aggregates.Snapshot.Strategies;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using EventFly.Definitions;
using EventFly.DependencyInjection;
using EventFly.Exceptions;
using EventFly.Extensions;
using EventFly.Metadata;
using EventFly.Permissions;
using Microsoft.Extensions.DependencyInjection;
using SnapshotMetadata = EventFly.Aggregates.Snapshot.SnapshotMetadata;

namespace EventFly.Aggregates
{
    public abstract class EventSourcedAggregateRoot<TAggregate, TIdentity, TAggregateState> : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TAggregate : EventSourcedAggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregateState : AggregateState<TAggregate,TIdentity, IMessageApplier<TAggregate,TIdentity>>, new()
        where TIdentity : IIdentity
    {
        private static readonly IReadOnlyDictionary<Type, Action<TAggregateState, IAggregateEvent>> ApplyMethodsFromState = typeof(TAggregateState).GetAggregateStateEventApplyMethods<TAggregate, TIdentity, TAggregateState>();
        private static readonly IReadOnlyDictionary<Type, Action<TAggregateState, IAggregateSnapshot>> HydrateMethodsFromState =  typeof(TAggregateState).GetAggregateSnapshotHydrateMethods<TAggregate, TIdentity, TAggregateState>();
        private static readonly IAggregateName AggregateName = typeof(TAggregate).GetAggregateName();
        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(100);
        private ICommand PinnedCommand { get; set; }
        private object PinnedReply { get; set; }
        private readonly IEventDefinitions _eventDefinitionService;
        private readonly ISnapshotDefinitions _snapshotDefinitionService;
        private ISnapshotStrategy SnapshotStrategy { get; set; } = SnapshotNeverStrategy.Instance;
        public TAggregateState State { get; }
        public IAggregateName Name => AggregateName;
        public override string PersistenceId { get; }
        public TIdentity Id { get; }
        public long Version { get; protected set; }
        public bool IsNew => Version <= 0;
        public override Recovery Recovery => new Recovery(SnapshotSelectionCriteria.Latest);
        private AggregateRootSettings Settings { get; }

        protected readonly IServiceProvider _serviceProvider;
        protected IServiceScope _scope;

        public ISecurityContext SecurityContext { get; private set; }


        protected EventSourcedAggregateRoot(TIdentity id)
        {
            _serviceProvider = Context.System.GetExtension<ServiceProviderHolder>().ServiceProvider;

            Settings = new AggregateRootSettings(Context.System.Settings.Config);
            
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
                    State = new TAggregateState();
                }
                catch(Exception exception)
                {
                    Context.GetLogger().Error(exception,"Unable to activate AggregateState of Type={0} for AggregateRoot of Name={1}.",typeof(TAggregateState).PrettyPrint(), Name);
                }

            }

            PinnedCommand = null;
            var app = _serviceProvider.GetService<IApplicationDefinition>();
            _eventDefinitionService = app.Events;
            _snapshotDefinitionService = app.Snapshots;
            Id = id;
            PersistenceId = id.Value;
            SetSourceIdHistory(100);
            
            if (Settings.UseDefaultSnapshotRecover)
            {
                Recover<SnapshotOffer>(Recover);
            }

            Command<SaveSnapshotSuccess>(SnapshotStatus);
            Command<SaveSnapshotFailure>(SnapshotStatus);

            if (Settings.UseDefaultEventRecover)
            {
                Recover<ICommittedEvent<TAggregate, TIdentity, IAggregateEvent<TIdentity>>>(Recover);
                Recover<RecoveryCompleted>(Recover);
            }
            
            this.InitAggregateReceivers();

            SetReceiveTimeout(Settings.SetReceiveTimeout);
            Command<ReceiveTimeout>(Timeout);

        }

        protected override void PreStart()
        {
            base.PreStart();

            _scope ??= _serviceProvider.CreateScope();
        }

        protected override void PostStop()
        {
            base.PostStop();
            _scope?.Dispose();
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

        public virtual void Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IEventMetadata metadata = null)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            var committedEvent = From(aggregateEvent, Version, metadata);
            Persist(committedEvent, ApplyCommittedEvent);
        }

        
        
        public virtual void EmitAll(params IAggregateEvent<TIdentity>[] aggregateEvents)
        {
            var version = Version;
            
            var committedEvents = new List<object>();
            foreach (var aggregateEvent in aggregateEvents)
            {
                var committedEvent = FromObject(aggregateEvent, version + 1);
                committedEvents.Add(committedEvent);
                version++;
            }

            PersistAll(committedEvents, ApplyObjectCommittedEvent);
        }
         

        protected virtual object FromObject(object aggregateEvent, long version, IEventMetadata metadata = null)
        {
            if (aggregateEvent is IAggregateEvent)
            {
                var eventDefinition = _eventDefinitionService.GetDefinition(aggregateEvent.GetType());
                var aggregateSequenceNumber = version + 1;
                var eventId = EventId.NewDeterministic(
                    GuidFactories.Deterministic.Namespaces.Events,
                    $"{Id.Value}-v{aggregateSequenceNumber}");
                var now = DateTimeOffset.UtcNow;
                var eventMetadata = new EventMetadata(PinnedCommand.Metadata)
                {
                    Timestamp = now,
                    AggregateSequenceNumber = aggregateSequenceNumber,
                    AggregateName = Name.Value,
                    AggregateId = Id.Value,
                    EventId = eventId,
                    EventName = eventDefinition.Name,
                    EventVersion = eventDefinition.Version
                };

                eventMetadata.AddOrUpdateValue(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
                if (metadata != null)
                {
                    eventMetadata.AddRange(metadata);
                }
                var genericType = typeof(CommittedEvent<,,>)
                    .MakeGenericType(typeof(TAggregate), typeof(TIdentity),aggregateEvent.GetType());


                var committedEvent = Activator.CreateInstance(
                    genericType,
                    Id,
                    aggregateEvent,
                    eventMetadata,
                    now,
                    aggregateSequenceNumber);

                return committedEvent;
            }
            
            throw new InvalidOperationException("could not perform the required mapping for committed event.");
            
        }
        
        public virtual CommittedEvent<TAggregate, TIdentity, TAggregateEvent> From<TAggregateEvent>(TAggregateEvent aggregateEvent,
            long version, IEventMetadata metadata = null)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }
            var eventDefinition = _eventDefinitionService.GetDefinition(aggregateEvent.GetType());
            var aggregateSequenceNumber = version + 1;
            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{Id.Value}-v{aggregateSequenceNumber}");
            var now = DateTimeOffset.UtcNow;
            var eventMetadata = new EventMetadata(PinnedCommand.Metadata)
            {
                Timestamp = now,
                AggregateSequenceNumber = aggregateSequenceNumber,
                AggregateName = Name.Value,
                AggregateId = Id.Value,
                EventId = eventId,
                EventName = eventDefinition.Name,
                EventVersion = eventDefinition.Version
            };

            eventMetadata.AddOrUpdateValue(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }
            
            var committedEvent = new CommittedEvent<TAggregate, TIdentity, TAggregateEvent>(Id, aggregateEvent,eventMetadata,now,aggregateSequenceNumber);
            return committedEvent;
        }
        protected virtual IAggregateSnapshot<TAggregate, TIdentity> CreateSnapshot()
        {
            Log.Warning("Aggregate of Name={0}, and Id={1}; attempted to create a snapshot, override the {2}() method to get snapshotting to function.", Name, Id, nameof(CreateSnapshot));
            return null;
        }

        protected void  ApplyCommittedEvent<TAggregateEvent>(ICommittedEvent<TAggregate, TIdentity, TAggregateEvent> committedEvent)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            var applyMethods = GetEventApplyMethods(committedEvent.AggregateEvent);
            applyMethods(committedEvent.AggregateEvent);

            Log.Info("Aggregate of Name={0}, and Id={1}; committed and applied an AggregateEvent of Type={2}.", Name, Id, typeof(TAggregateEvent).PrettyPrint());

            Version++;

            var domainEvent = new DomainEvent<TAggregate,TIdentity,TAggregateEvent>(Id, committedEvent.AggregateEvent,committedEvent.EventMetadata,committedEvent.Timestamp,Version);

            Publish(domainEvent);
            ReplyIfAvailable();
            
            if (SnapshotStrategy.ShouldCreateSnapshot(this))
            {
                var aggregateSnapshot = CreateSnapshot();
                if (aggregateSnapshot != null)
                {
                    var snapshotDefinition = _snapshotDefinitionService.GetDefinition(aggregateSnapshot.GetType());
                    var snapshotMetadata = new SnapshotMetadata
                    {
                        AggregateId = Id.Value,
                        AggregateName = Name.Value,
                        AggregateSequenceNumber = Version,
                        SnapshotName = snapshotDefinition.Name,
                        SnapshotVersion = snapshotDefinition.Version
                    };

                    var committedSnapshot =
                        new CommittedSnapshot<TAggregate, TIdentity, IAggregateSnapshot<TAggregate, TIdentity>>(
                            Id,
                            aggregateSnapshot,
                            snapshotMetadata,
                            committedEvent.Timestamp, Version);

                    SaveSnapshot(committedSnapshot);
                }
            }
        }

        private void ApplyObjectCommittedEvent(object committedEvent)
        {
            try
            {
                var method = GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.IsFamily || m.IsPublic)
                    .Single(m => m.Name.Equals("ApplyCommittedEvent"));

                var genericMethod = method.MakeGenericMethod(committedEvent.GetType().GenericTypeArguments[2]);

                genericMethod.Invoke(this, new[] {committedEvent});
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Aggregate of Name={0}, and Id={1}; tried to invoke Method={2} with object Type={3}.",Name, Id, nameof(ApplyCommittedEvent), committedEvent.GetType().PrettyPrint());
            }
        }

        protected virtual void Publish<TEvent>(TEvent aggregateEvent)
        {
            Context.System.EventStream.Publish(aggregateEvent);
            Log.Info("Aggregate of Name={0}, and Id={1}; published DomainEvent of Type={2}.",Name, Id, typeof(TEvent).PrettyPrint());
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            if (message is ICommand command)
            {
                if(IsNew || Id.Equals(command.GetAggregateId()))
                    PinnedCommand = command;
            }

            return base.AroundReceive(receive, message);
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

        protected Action<IAggregateEvent> GetEventApplyMethods<TAggregateEvent>(TAggregateEvent aggregateEvent)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            var eventType = aggregateEvent.GetType();

            Action<TAggregateState, IAggregateEvent> applyMethod;
            if (!ApplyMethodsFromState.TryGetValue(eventType, out applyMethod))
                throw new NotImplementedException($"AggregateState of Type={State.GetType().PrettyPrint()} does not have an 'Apply' method that takes in an aggregate event of Type={eventType.PrettyPrint()} as an argument.");

            var aggregateApplyMethod = applyMethod.Bind(State);

            return aggregateApplyMethod;
        }

        protected Action<IAggregateSnapshot> GetSnapshotHydrateMethods<TAggregateSnapshot>(TAggregateSnapshot aggregateEvent)
            where TAggregateSnapshot : class, IAggregateSnapshot<TAggregate, TIdentity>
        {
            var snapshotType = aggregateEvent.GetType();

            Action<TAggregateState, IAggregateSnapshot> hydrateMethod;
            if (!HydrateMethodsFromState.TryGetValue(snapshotType, out hydrateMethod))
                throw new NotImplementedException($"AggregateState of Type={State.GetType().PrettyPrint()} does not have a 'Hydrate' method that takes in an aggregate snapshot of Type={snapshotType.PrettyPrint()} as an argument.");
            


            var snapshotHydrateMethod = hydrateMethod.Bind(State);

            return snapshotHydrateMethod;
        }

        protected virtual void ApplyEvent(IAggregateEvent<TIdentity> aggregateEvent)
        {
            var eventApplier = GetEventApplyMethods(aggregateEvent);

            eventApplier(aggregateEvent);

            Version++;
        }

        protected virtual void HydrateSnapshot(IAggregateSnapshot<TAggregate, TIdentity> aggregateSnapshot, long version)
        {
            var snapshotHydrater = GetSnapshotHydrateMethods(aggregateSnapshot);

            snapshotHydrater(aggregateSnapshot);

            Version = version;
        }

        protected virtual bool Recover(ICommittedEvent<TAggregate, TIdentity, IAggregateEvent<TIdentity>> committedEvent)
        {
            try
            {
                Log.Debug("Aggregate of Name={0}, Id={1}, and Version={2}, is recovering with CommittedEvent of Type={3}.", Name, Id, Version, committedEvent.GetType().PrettyPrint());
                ApplyEvent(committedEvent.AggregateEvent);
            }
            catch(Exception exception)
            {
                Log.Error(exception,"Aggregate of Name={0}, Id={1}; while recovering with event of Type={2} caused an exception.", Name, Id, committedEvent.GetType().PrettyPrint());
                return false;
            }

            return true;
        }

        protected virtual bool Recover(SnapshotOffer aggregateSnapshotOffer)
        {
            try
            {
                Log.Debug("Aggregate of Name={0}, and Id={1}; has received a SnapshotOffer of Type={2}.", Name, Id, aggregateSnapshotOffer.Snapshot.GetType().PrettyPrint());
                var comittedSnapshot = aggregateSnapshotOffer.Snapshot as CommittedSnapshot<TAggregate,TIdentity, IAggregateSnapshot<TAggregate, TIdentity>>;
                if (comittedSnapshot != null) HydrateSnapshot(comittedSnapshot.AggregateSnapshot, aggregateSnapshotOffer.Metadata.SequenceNr);
            }
            catch (Exception exception)
            {
                Log.Error(exception,"Aggregate of Name={0}, Id={1}; recovering with snapshot of Type={2} caused an exception.", Name, Id, aggregateSnapshotOffer.Snapshot.GetType().PrettyPrint());

                return false;
            }

            return true;
        }

        protected virtual void SetSnapshotStrategy(ISnapshotStrategy snapshotStrategy)
        {
            if (snapshotStrategy != null)
            {
                SnapshotStrategy = snapshotStrategy;
            }
        }
        protected virtual bool SnapshotStatus(SaveSnapshotSuccess snapshotSuccess)
        {
            Log.Debug("Aggregate of Name={0}, and Id={1}; saved a snapshot at Version={2}.", Name, Id, snapshotSuccess.Metadata.SequenceNr);
            DeleteSnapshots(new SnapshotSelectionCriteria(snapshotSuccess.Metadata.SequenceNr-1));
            return true;
        }

        protected virtual bool SnapshotStatus(SaveSnapshotFailure snapshotFailure)
        {
            Log.Error(snapshotFailure.Cause,"Aggregate of Name={0}, and Id={1}; failed to save snapshot at Version={2}.", Name, Id, snapshotFailure.Metadata.SequenceNr);
            return true;
        }


        protected virtual bool Recover(RecoveryCompleted recoveryCompleted)
        {
            Log.Debug("Aggregate of Name={0}, and Id={1}; has completed recovering from it's event journal at Version={2}.", Name, Id, Version);
            return true;
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
        
        protected void Command<TCommand, TCommandHandler>(Predicate<TCommand> shouldHandle = null)
            where TCommand : ICommand<TIdentity>
            where TCommandHandler : CommandHandler<TAggregate, TIdentity, TCommand>
        {
            try
            {
                Command(x =>
                {
                    var handler = _scope.ServiceProvider.GetService<TCommandHandler>() ?? (TCommandHandler)Activator.CreateInstance(typeof(TCommandHandler));
                    try
                    {
                        var result = handler.Handle(this as TAggregate, x);
                        Context.Sender.Tell(result);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Context.Sender.Tell(new UnauthorizedAccessResult());
                    }
                },shouldHandle);
            }
            catch (Exception exception)
            {
                Log.Error(exception,"Unable to activate CommandHandler of Type={0} for Aggregate of Type={1}.",typeof(TCommandHandler).PrettyPrint(), typeof(TAggregate).PrettyPrint());
            }

        }
        
        protected void Command<TCommand, TResult>(Func<TCommand,TResult> handler)
            where TCommand : ICommand<TIdentity>
        {
            Command(x =>
                {
                    var result = handler(x);
                    Context.Sender.Tell(result);

                },
                (Predicate<TCommand>) (command =>
                {
                    string userId = command.Metadata.ContainsKey(MetadataKeys.UserId) ? command.Metadata?.UserId : null;
                    SecurityContext = new SecurityContext(userId, _serviceProvider.GetService<ISecurityService>());
                    return true;
                }));
        }
        
    }

}
