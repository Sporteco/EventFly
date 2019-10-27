using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using EventFly.DependencyInjection;
using EventFly.Exceptions;
using EventFly.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.DomainService
{
    public abstract class DomainService<TDomainService, TIdentity> : ReceiveActor, IDomainService<TIdentity>
        where TDomainService : DomainService<TDomainService, TIdentity>
        where TIdentity : IIdentity
    {
        protected ILoggingAdapter Logger { get; set; }

        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(100);
        protected override void PreStart()
        {
            base.PreStart();
            _scope = _serviceProvider.CreateScope();
        }

        protected override void PostStop()
        {
            base.PostStop();
            _scope.Dispose();
        }

        public async Task<ExecutionResult> PublishCommandAsync<TCommandIdentity, TExecutionResult>(ICommand<TCommandIdentity, TExecutionResult> command) where TCommandIdentity : IIdentity where TExecutionResult : IExecutionResult
        {
            var result = CommandValidationHelper.ValidateCommand(command, _serviceProvider);
            if (!result.IsValid) return new FailedValidationExecutionResult(result);


            if (_pinnedEvent != null)
            {
                command.Metadata.Merge(_pinnedEvent.Metadata);
            }

            if (!command.Metadata.CorrellationIds.Contains(Id.Value))
            {
                command.Metadata.CorrellationIds = new List<string>(command.Metadata.CorrellationIds) { Id.Value };
            }

            var bus = _scope.ServiceProvider.GetRequiredService<ICommandBus>();

            return await bus.Publish(command);
        }

        public TIdentity Id { get; }

        private readonly IServiceProvider _serviceProvider;
        
        private IServiceScope _scope;

        private DomainServiceSettings Settings { get; }

        private IDomainEvent _pinnedEvent;

        protected DomainService()
        {
            Logger = Context.GetLogger();

            _serviceProvider = Context.System.GetExtension<ServiceProviderHolder>().ServiceProvider;

            Settings = new DomainServiceSettings(Context.System.Settings.Config);
            var idValue = Context.Self.Path.Name;
            Id = (TIdentity)Activator.CreateInstance(typeof(TIdentity), idValue);

            if (Id == null)
            {
                throw new InvalidOperationException(
                    $"Identity for DomainService '{Id.GetType().PrettyPrint()}' could not be activated.");
            }

            if ((this as TDomainService) == null)
            {
                throw new InvalidOperationException(
                    $"DomainService specifies Type={typeof(TDomainService).PrettyPrint()} as generic argument, it should be its own type.");
            }

            if (Settings.AutoReceive)
            {
                InitReceives();
                InitAsyncReceives();
            }

        }

        public void InitReceives()
        {
            var type = GetType();

            var subscriptionTypes =
                type
                    .GetDomainServiceEventSubscriptionTypes();

            
            var methods = type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Handle")
                        return false;

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
                    if (mi.Name != "CommandInternal") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1
                        && parameters[0].ParameterType.Name.Contains("Func");
                })
                .First();

            foreach (var subscriptionType in subscriptionTypes)
            {
                Context.System.EventStream.Subscribe(Self,subscriptionType);
                var funcType = typeof(Func<,>).MakeGenericType(subscriptionType, typeof(bool));
                var subscriptionFunction = Delegate.CreateDelegate(funcType, this, methods[subscriptionType]);
                var actorReceiveMethod = method.MakeGenericMethod(subscriptionType);

                actorReceiveMethod.Invoke(this, new object[] { subscriptionFunction });
            }
        }
        public void InitAsyncReceives()
        {
            var type = GetType();

            var subscriptionTypes =
                type
                    .GetAsyncDomainEventSubscriberSubscriptionTypes();

            var methods = type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "HandleAsync")
                        return false;

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
                    if (mi.Name != "CommandInternal") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 2
                        && parameters[0].ParameterType.Name.Contains("Func");
                })
                .First();

            foreach (var subscriptionType in subscriptionTypes)
            {
                Context.System.EventStream.Subscribe(Self,subscriptionType);
                var funcType = typeof(Func<,>).MakeGenericType(subscriptionType, typeof(Task));
                var subscriptionFunction = Delegate.CreateDelegate(funcType, this, methods[subscriptionType]);
                var actorReceiveMethod = method.MakeGenericMethod(subscriptionType);

                actorReceiveMethod.Invoke(this, new[] { subscriptionFunction, (object)null });
            }
        }
        protected void CommandInternal<T>(Func<T, bool> handler)
        {
            Receive<T>(e =>
            {
                if (e is IDomainEvent @event)
                {
                    _pinnedEvent = @event;
                }
                handler(e);
            });
        }

        protected void CommandInternal<T>(Func<T, Task> handler, object item)
        {
            ReceiveAsync<T>(e =>
            {
                if (e is IDomainEvent @event)
                {
                    _pinnedEvent = @event;
                }
                return handler(e);
            });
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

    }
}