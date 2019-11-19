using Akka.Event;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.Core;
using EventFly.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace EventFly.TestFixture
{
    public static class BddTestHelper
    {
        public static void Init(TestKit testKit)
        {
            _testProbe = testKit.CreateTestProbe("event-probe");
            _serviceProvider = testKit.Sys.GetExtension<ServiceProviderHolder>().ServiceProvider;
            _commandBus = _serviceProvider.GetRequiredService<ICommandBus>();
            _eventStream = testKit.Sys.EventStream;
        }

        public static void EmptyStep(this String _) { }

        public static String Do(this String stepName, ICommand command)
        {
            _commandBus.Publish(command);

            return stepName;
        }

        public static String DoMany<TAggregateState, TIdentity, TAggregate>(this String stepName, TIdentity aggregateId, Func<TAggregateState, IEnumerable<ICommand>> commandsBuilder)
            where TAggregateState : class, IAggregateState<TIdentity>
            where TIdentity : IIdentity
            where TAggregate : IAggregateRoot
        {
            var aggregate = _serviceProvider.GetRequiredService<IAggregateState<TIdentity>>();
            aggregate.LoadState(aggregateId).GetAwaiter().GetResult();
            var commands = commandsBuilder.Invoke(aggregate as TAggregateState);
            foreach (var command in commands) _commandBus.Publish(command);

            return stepName;
        }

        public static String Do<TAggregateState, TIdentity, TAggregate>(this String stepName, TIdentity aggregateId, Func<TAggregateState, ICommand> commandBuilder)
            where TAggregateState : class, IAggregateState<TIdentity>, new()
            where TIdentity : IIdentity
            where TAggregate : IAggregateRoot
        {
            var aggregate = _serviceProvider.GetRequiredService<TAggregateState>();
            aggregate.LoadState(aggregateId).GetAwaiter().GetResult();
            var command = commandBuilder.Invoke(aggregate);
            _commandBus.Publish(command).GetAwaiter().GetResult();

            return stepName;
        }

        public static String Expect<TAggregateEvent>(this String stepName)
            where TAggregateEvent : IAggregateEvent
        {
            var domainEventType = GetDomainEventType(typeof(TAggregateEvent));
            _eventStream.Subscribe(_testProbe, domainEventType);
            CallAkkaExpect(domainEventType);

            return stepName;
        }

        public static String DoAndExpect<TAggregateEvent>(this String stepName, ICommand command)
            where TAggregateEvent : IAggregateEvent
        {
            var domainEventType = GetDomainEventType(typeof(TAggregateEvent));
            _eventStream.Subscribe(_testProbe, domainEventType);
            _commandBus.Publish(command);
            CallAkkaExpect(domainEventType);

            return stepName;
        }

        public static String Validate<TAggregateState, TIdentity>(this String stepName, TIdentity aggregateId, Action<TAggregateState> validator)
            where TAggregateState : class, IAggregateState<TIdentity>
            where TIdentity : IIdentity
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var aggregate = scope.ServiceProvider.GetService<IAggregateState<TIdentity>>();
                aggregate.LoadState(aggregateId).GetAwaiter().GetResult();
                validator.Invoke(aggregate as TAggregateState);
            }

            return stepName;
        }

        private static IServiceProvider _serviceProvider;
        private static TestProbe _testProbe;
        private static ICommandBus _commandBus;
        private static EventStream _eventStream;
        private static EventWaitHandle _wait;

        private static Type GetDomainEventType(Type aggregateEventType)
        {
            var idType = aggregateEventType.BaseType.GenericTypeArguments.Single();
            return typeof(DomainEvent<,>).MakeGenericType(idType, aggregateEventType);
        }

        private static void CallAkkaExpect(Type domainEventType)
        {
            var assertType = typeof(Action<>).MakeGenericType(domainEventType);
            var signalMethodDef = typeof(BddTestHelper).GetMethod(nameof(BddTestHelper.Signal), BindingFlags.Static | BindingFlags.NonPublic);
            var signalMethod = signalMethodDef.MakeGenericMethod(domainEventType);
            var expectMethodParams = new[] { typeof(Action<>), typeof(TimeSpan?), typeof(String) };
            var expectMethodDef = typeof(TestProbe).GetMethods().Single(x =>
                x.Name == nameof(TestProbe.ExpectMsg) &&
                x.IsGenericMethodDefinition &&
                x.GetParameters().Length == 3 &&
                x.GetParameters()[0].ParameterType.IsGenericType &&
                x.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Action<>) &&
                x.GetParameters()[1].ParameterType == typeof(TimeSpan?) &&
                x.GetParameters()[2].ParameterType == typeof(String));
            var expectMethod = expectMethodDef.MakeGenericMethod(domainEventType);
            var timeout = Debugger.IsAttached ? TimeSpan.FromSeconds(120) : (TimeSpan?)null;

            using (_wait = new AutoResetEvent(true))
            {
                var assert = Delegate.CreateDelegate(assertType, signalMethod);
                expectMethod.Invoke(_testProbe, new Object[] { assert, timeout, null });
            }
        }

        private static void Signal<T>(T _)
        {
            _eventStream.Unsubscribe(_testProbe, typeof(T));
            _wait.Set();
        }
    }
}