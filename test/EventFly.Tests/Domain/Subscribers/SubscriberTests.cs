// The MIT License (MIT)
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

using System.ComponentModel;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using EventFly.Commands;
using EventFly.DependencyInjection;
using EventFly.Infrastructure.DependencyInjection;
using EventFly.Tests.Data.Abstractions;
using EventFly.Tests.Data.Abstractions.Commands;
using EventFly.Tests.Data.Abstractions.Events;
using EventFly.Tests.Data.Application.Sagas.Test;
using EventFly.Tests.Data.Domain;
using EventFly.Tests.Data.Domain.Subscribers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Tests.Domain.Subscribers
{
    [Category(Categories.Domain)]
    [Collection(Collections.Only)]
    public class SubscriberTests : TestKit
    {
        public SubscriberTests(ITestOutputHelper testOutputHelper) : base(Tests.Configuration.Default, "subscriber-tests", testOutputHelper)
        {
            Sys.RegisterDependencyResolver(
                new ServiceCollection()
                .AddEventFly(Sys)
                .WithContext<TestContext>()
                .Services
                .AddScoped<TestSaga>()
                .BuildServiceProvider()
                .UseEventFly()
            );
        }

        [Fact]
        public void Subscriber_ReceivedEvent_FromAggregatesEmit()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(TestSubscribedEventHandled<TestCreatedEvent>));
            Sys.ActorOf(Props.Create(() => new TestAggregateSubscriber()), "test-subscriber");
            var bus = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<ICommandBus>();

            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new CreateTestCommand(aggregateId, commandId);
            bus.Publish(command).GetAwaiter().GetResult();

            eventProbe.ExpectMsg<TestSubscribedEventHandled<TestCreatedEvent>>(x => x.AggregateEvent.TestAggregateId == command.AggregateId);
        }

        [Fact]
        public void Subscriber_ReceivedAsyncEvent_FromAggregatesEmit()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(TestAsyncSubscribedEventHandled<TestCreatedEvent>));
            Sys.ActorOf(Props.Create(() => new TestAsyncAggregateSubscriber()), "test-subscriber");
            var bus = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<ICommandBus>();

            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new CreateTestCommand(aggregateId, commandId);
            bus.Publish(command).GetAwaiter().GetResult();

            eventProbe.ExpectMsg<TestAsyncSubscribedEventHandled<TestCreatedEvent>>(x => x.AggregateEvent.TestAggregateId == command.AggregateId);
        }

        [Fact]
        public void Subscriber_ReceivedAsyncAggregateEvent_FromAggregatesEmit()
        {
            var eventProbe = CreateTestProbe("event-probe");
            Sys.EventStream.Subscribe(eventProbe, typeof(TestManySubscribedEventHandled));
            Sys.ActorOf(Props.Create(() => new TestAsyncManyEventSubscriber()), "test-subscriber");
            var bus = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<ICommandBus>();

            var aggregateId = TestAggregateId.New;
            var commandId = CommandId.New;
            var command = new CreateTestCommand(aggregateId, commandId);
            bus.Publish(command).GetAwaiter().GetResult();

            eventProbe.ExpectMsg<TestManySubscribedEventHandled>(x =>
                x.DomainEvent.GetIdentity().Value == aggregateId && x.DomainEvent.EventType == typeof(TestCreatedEvent)
            );
        }
    }
}