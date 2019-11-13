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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFly.Aggregates
{
    public abstract class AggregateRoot<TAggregate, TIdentity, TAggregateState> : ReceiveActor, IAggregateRoot<TIdentity>
        where TAggregate : AggregateRoot<TAggregate, TIdentity, TAggregateState>
        where TAggregateState : IAggregateState<TIdentity>
        where TIdentity : IIdentity
    {
        public TAggregateState State { get; private set; }
        public IAggregateName Name => _aggregateName;
        public Int64 Version => 1;
        public Boolean IsNew => false;
        public TIdentity Id { get; }

        public Boolean HasSourceId(ISourceId sourceId)
        {
            return !sourceId.IsNone() && _previousSourceIds.Any(s => s.Value == sourceId.Value);
        }

        public IIdentity GetIdentity() => Id;

        public virtual Task Emit<TAggregateEvent>(TAggregateEvent aggregateEvent, IEventMetadata metadata = null)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            var committedEvent = From(aggregateEvent, Version, metadata);
            ApplyCommittedEvent(committedEvent);

            return Task.CompletedTask;
        }

        public virtual CommittedEvent<TAggregate, TIdentity, TAggregateEvent> From<TAggregateEvent>(TAggregateEvent aggregateEvent, Int64 version, IEventMetadata metadata = null)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            if (aggregateEvent == null) throw new ArgumentNullException(nameof(aggregateEvent));

            var eventDefinition = _eventDefinitionService.GetDefinition(aggregateEvent.GetType());
            var aggregateSequenceNumber = version + 1;
            var eventId = EventId.NewDeterministic(GuidFactories.Deterministic.Namespaces.Events, $"{Id.Value}-v{aggregateSequenceNumber}");
            var now = DateTimeOffset.UtcNow;
            var eventMetadata = new EventMetadata(_pinnedCommand.Metadata)
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
            if (metadata != null) eventMetadata.AddRange(metadata);

            return new CommittedEvent<TAggregate, TIdentity, TAggregateEvent>(Id, aggregateEvent, eventMetadata, now, aggregateSequenceNumber);
        }

        public Boolean Timeout(ReceiveTimeout _)
        {
            Log.Debug("Aggregate of Name={0}, and Id={1}; has received a timeout message and will stop.", Name, Id);
            Context.Stop(Self);
            return true;
        }

        public override void AroundPreRestart(Exception cause, Object message)
        {
            Log.Error(cause, "Aggregate of Name={0}, and Id={1}; has experienced an error and will now restart", Name, Id);
            base.AroundPreRestart(cause, message);
        }

        public override String ToString() => $"{GetType().PrettyPrint()} v{Version}";

        #region Stuff

        protected SecurityContext SecurityContext { get; private set; }
        protected readonly ILoggingAdapter Log = Context.GetLogger();

        protected AggregateRoot(TIdentity id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if ((this as TAggregate) == null)
            {
                var message = $"Aggregate {Name} specifies Type={typeof(TAggregate).PrettyPrint()} as generic argument, it should be its own type.";
                throw new InvalidOperationException(message);
            }

            _serviceProvider = Context.System.GetExtension<ServiceProviderHolder>().ServiceProvider;            
            _pinnedCommand = null;
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
                    State = (TAggregateState)_scope.ServiceProvider.GetService<IAggregateState<TIdentity>>();
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
                Context.Sender.Tell(new UnhandledExceptionCommandResult(exception.Message, exception.StackTrace));
            }
        }

        protected void SetSourceIdHistory(Int32 count)
        {
            _previousSourceIds = new CircularBuffer<ISourceId>(count);
        }

        protected void ApplyCommittedEvent<TAggregateEvent>(ICommittedEvent<TAggregate, TIdentity, TAggregateEvent> committedEvent)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            Log.Info("Aggregate of Name={0}, and Id={1}; committed and applied an AggregateEvent of Type={2}.", Name, Id, typeof(TAggregateEvent).PrettyPrint());

            var aggEvent = committedEvent.AggregateEvent;
            var meta = committedEvent.EventMetadata;
            var timestamp = committedEvent.Timestamp;
            var domainEvent = new DomainEvent<TAggregate, TIdentity, TAggregateEvent>(Id, aggEvent, meta, timestamp, Version);

            Publish(domainEvent);
            ReplyIfAvailable();
        }

        protected virtual void Publish<TEvent>(TEvent aggregateEvent)
        {
            Context.System.EventStream.Publish(aggregateEvent);
            Log.Info("Aggregate of Name={0}, and Id={1}; published DomainEvent of Type={2}.", Name, Id, typeof(TEvent).PrettyPrint());
        }

        protected override Boolean AroundReceive(Receive receive, Object message)
        {
            if (message is Command<TIdentity> command)
            {
                if (IsNew || Id.Equals(command.AggregateId)) _pinnedCommand = command;
                else _pinnedCommand = null;

                var userId = command.Metadata.ContainsKey(MetadataKeys.UserId) ? command.Metadata?.UserId : null;
                SecurityContext = new SecurityContext(userId, _serviceProvider.GetService<ISecurityService>());
            }

            var result = base.AroundReceive(receive, message);
            return result;
        }

        protected virtual void Reply(Object replyMessage)
        {
            if (!Sender.IsNobody()) _pinnedReply = replyMessage;
        }

        protected virtual void ReplyFailure(Object replyMessage)
        {
            if (!Sender.IsNobody()) Context.Sender.Tell(replyMessage);
        }

        protected virtual void ReplyIfAvailable()
        {
            if (_pinnedReply != null) Sender.Tell(_pinnedReply);
            _pinnedReply = null;
        }

        protected override void Unhandled(Object message)
        {
            Log.Warning("Aggregate of Name={0}, and Id={1}; has received an unhandled message of Type={2}.", Name, Id, message.GetType().PrettyPrint());
            base.Unhandled(message);
        }

        protected IEnumerable<IAggregateEvent<TIdentity>> Events(params IAggregateEvent<TIdentity>[] events) => events.ToList();

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

        private Task LoadState() => State.LoadState(Id);

        private IServiceScope _scope;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventDefinitions _eventDefinitionService;
        private static readonly IAggregateName _aggregateName = typeof(TAggregate).GetAggregateName();
        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(100);
        private ICommand _pinnedCommand;
        private Object _pinnedReply;

        #endregion
    }
}