using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
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

namespace EventFly.Aggregates
{
    public abstract class AggregateRoot<TAggregate, TIdentity, TAggregateState> : ReceiveActor, IAggregateRoot<TIdentity>
       where TAggregate : AggregateRoot<TAggregate, TIdentity, TAggregateState>
       where TAggregateState : IAggregateState<TIdentity>
       where TIdentity : IIdentity
    {
        private readonly IEventDefinitions _eventDefinitionService;

        private static readonly IAggregateName AggregateName = typeof(TAggregate).GetAggregateName();
        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(100);
        private ICommand PinnedCommand { get; set; }
        private object PinnedReply { get; set; }
        public TAggregateState State { get; private set; }
        public IAggregateName Name => AggregateName;
        public long Version => 1;
        public bool IsNew => false;
        public TIdentity Id { get; }
        protected SecurityContext SecurityContext { get; private set; }

        private IServiceScope _scope;
        private readonly IServiceProvider _serviceProvider;

        protected readonly ILoggingAdapter Log = Context.GetLogger();
        protected AggregateRoot(TIdentity id)
        {
            _serviceProvider = Context.System.GetExtension<ServiceProviderHolder>().ServiceProvider;
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if ((this as TAggregate) == null)
            {
                throw new InvalidOperationException(
                    $"Aggregate {Name} specifies Type={typeof(TAggregate).PrettyPrint()} as generic argument, it should be its own type.");
            }

            PinnedCommand = null;
            _eventDefinitionService = _serviceProvider.GetService<IApplicationDefinition>().Events;
            Id = id;
            SetSourceIdHistory(100);
            this.InitAggregateReceivers();
        }

        protected override void PreStart()
        {
            base.PreStart();

            _scope ??= _serviceProvider.CreateScope();

            if (State == null)
            {
                try
                {
                    State = (TAggregateState) _scope.ServiceProvider.GetService<IAggregateState<TIdentity>>();
                }
                catch (Exception exception)
                {
                    Context.GetLogger().Error(exception, "Unable to activate AggregateState of Type={0} for AggregateRoot of Name={1}.", typeof(TAggregateState).PrettyPrint(), Name);
                }
            }

            LoadState().GetAwaiter().GetResult();
        }

        protected override void PostStop()
        {
            base.PostStop();
            _scope?.Dispose();
        }


        private Task LoadState()
        {
            return State.LoadState(Id);
        }

        


        protected void Command<TCommand>()
            where TCommand : ICommand<TIdentity>
        {
            try
            {
                ReceiveAsync<TCommand>(async cmd =>
                {
                    var handler = _scope.ServiceProvider.GetService<CommandHandler<TAggregate, TIdentity, TCommand>>();
                    var commandBus = _scope.ServiceProvider.GetService<ICommandBus>();
                    handler.Inject(commandBus);
                    var result = await handler.Handle(this as TAggregate, cmd);
                    Context.Sender.Tell(result);
                });
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Unable to activate CommandHandler for command of Type={0} for Aggregate of Type={1}.", typeof(TCommand).PrettyPrint(), typeof(TAggregate).PrettyPrint());
            }
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

        public virtual Task Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IEventMetadata metadata = null)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            var committedEvent = From(aggregateEvent, Version, metadata);

            ApplyCommittedEvent(committedEvent);

            return Task.CompletedTask;
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

            var committedEvent = new CommittedEvent<TAggregate, TIdentity, TAggregateEvent>(Id, aggregateEvent, eventMetadata, now, aggregateSequenceNumber);
            return committedEvent;
        }

        protected void ApplyCommittedEvent<TAggregateEvent>(ICommittedEvent<TAggregate, TIdentity, TAggregateEvent> committedEvent)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            Log.Info("Aggregate of Name={0}, and Id={1}; committed and applied an AggregateEvent of Type={2}.", Name, Id, typeof(TAggregateEvent).PrettyPrint());

            var domainEvent = new DomainEvent<TAggregate, TIdentity, TAggregateEvent>(Id, committedEvent.AggregateEvent, committedEvent.EventMetadata, committedEvent.Timestamp, Version);

            Publish(domainEvent);
            ReplyIfAvailable();
        }


        protected virtual void Publish<TEvent>(TEvent aggregateEvent)
        {
            Context.System.EventStream.Publish(aggregateEvent);
            Log.Info("Aggregate of Name={0}, and Id={1}; published DomainEvent of Type={2}.", Name, Id, typeof(TEvent).PrettyPrint());
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            if (message is Command<TIdentity> command)
            {
                if (IsNew || Id.Equals(command.AggregateId))
                    PinnedCommand = command;
                else
                    PinnedCommand = null;

                var userId = command.Metadata.ContainsKey(MetadataKeys.UserId) ? command.Metadata?.UserId : null;
                SecurityContext = new SecurityContext(userId, _serviceProvider.GetService<ISecurityService>());
            }

            var result = base.AroundReceive(receive, message);
            return result;
        }

        protected virtual void Reply(object replyMessage)
        {
            if (!Sender.IsNobody())
            {
                PinnedReply = replyMessage;
            }
        }

        protected virtual void ReplyFailure(object replyMessage)
        {
            if (!Sender.IsNobody())
            {
                Context.Sender.Tell(replyMessage);
            }
        }

        protected virtual void ReplyIfAvailable()
        {
            if (PinnedReply != null)
                Sender.Tell(PinnedReply);

            PinnedReply = null;
        }

        protected override void Unhandled(object message)
        {
            Log.Warning("Aggregate of Name={0}, and Id={1}; has received an unhandled message of Type={2}.", Name, Id, message.GetType().PrettyPrint());
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

        protected void Command<TCommand, TResult>(Func<TCommand, Task<TResult>> handler)
            where TCommand : ICommand<TIdentity>
            where TResult : IExecutionResult
        {
            ReceiveAsync(async x =>
                {
                    try
                    {
                        var result = await handler(x);
                        Context.Sender.Tell(result);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Context.Sender.Tell(new UnauthorizedAccessResult());
                    }

                }, (Predicate<TCommand>)null);
        }
    }
}
