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
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventFly.Domain
{
    public abstract class DomainService<TDomainService> : ReceiveActor, IDomainService
        where TDomainService : DomainService<TDomainService>
    {
        protected DomainService()
        {
            Logger = Context.GetLogger();

            var serviceProviderHolder = Context.System.GetExtension<ServiceProviderHolder>();
            if (serviceProviderHolder == null)
            {
                //todo Get rid of ServiceLocator anti-pattern?
                throw new ArgumentNullException("ServiceProviderHolder", "Implicit argument wasn't found in ActorContext.");
            }
            _serviceProvider = serviceProviderHolder.ServiceProvider;

            _settings = new DomainServiceSettings(Context.System.Settings.Config);

            if ((this as TDomainService) == null)
            {
                throw new InvalidOperationException($"DomainService specifies Type={typeof(TDomainService).PrettyPrint()} as generic argument, it should be its own type.");
            }

            if (_settings.AutoReceive)
            {
                InitReceives();
                InitAsyncReceives();
            }
        }

        public async Task<ExecutionResult> PublishCommandAsync<TCommandIdentity, TExecutionResult>(ICommand<TCommandIdentity, TExecutionResult> command)
            where TCommandIdentity : IIdentity
            where TExecutionResult : IExecutionResult
        {
            var result = CommandValidationHelper.ValidateCommand(command, _serviceProvider);
            if (!result.IsValid) return new FailedValidationExecutionResult(result);

            if (_pinnedEvent != null)
            {
                command.Metadata.Merge(_pinnedEvent.Metadata);
            }

            var bus = _scope.ServiceProvider.GetRequiredService<ICommandBus>();

            return await bus.Publish(command);
        }

        public void InitReceives()
        {
            var type = GetType();

            var subscriptionTypes = type.GetDomainServiceEventSubscriptionTypes();

            var methods = type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Handle") return false;
                    var parameters = mi.GetParameters();
                    return parameters.Length == 1;
                })
                .ToDictionary(mi => mi.GetParameters()[0].ParameterType, mi => mi);

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
                Context.System.EventStream.Subscribe(Self, subscriptionType);
                var funcType = typeof(Func<,>).MakeGenericType(subscriptionType, typeof(Boolean));
                var subscriptionFunction = Delegate.CreateDelegate(funcType, this, methods[subscriptionType]);
                var actorReceiveMethod = method.MakeGenericMethod(subscriptionType);

                actorReceiveMethod.Invoke(this, new Object[] { subscriptionFunction });
            }
        }

        public void InitAsyncReceives()
        {
            var type = GetType();

            var subscriptionTypes = type.GetAsyncDomainEventSubscriberSubscriptionTypes();

            var methods = type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "HandleAsync") return false;
                    var parameters = mi.GetParameters();
                    return parameters.Length == 1;
                })
                .ToDictionary(mi => mi.GetParameters()[0].ParameterType, mi => mi);

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
                Context.System.EventStream.Subscribe(Self, subscriptionType);
                var funcType = typeof(Func<,>).MakeGenericType(subscriptionType, typeof(Task));
                var subscriptionFunction = Delegate.CreateDelegate(funcType, this, methods[subscriptionType]);
                var actorReceiveMethod = method.MakeGenericMethod(subscriptionType);

                actorReceiveMethod.Invoke(this, new[] { subscriptionFunction, (Object)null });
            }
        }

        public Boolean HasSourceId(ISourceId sourceId)
        {
            return !sourceId.IsNone() && _previousSourceIds.Any(s => s.Value == sourceId.Value);
        }

        protected ILoggingAdapter Logger { get; set; }

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

        protected void CommandInternal<T>(Func<T, Boolean> handler)
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

        protected void CommandInternal<T>(Func<T, Task> handler, Object _)
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

        protected void SetSourceIdHistory(Int32 count)
        {
            _previousSourceIds = new CircularBuffer<ISourceId>(count);
        }

        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(100);
        private readonly IServiceProvider _serviceProvider;
        private IServiceScope _scope;
        private readonly DomainServiceSettings _settings;
        private IDomainEvent _pinnedEvent;
    }
}