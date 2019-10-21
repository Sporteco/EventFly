﻿// The MIT License (MIT)
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
using Akka.TestKit.Xunit2;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.DependencyInjection;

using EventFly.TestHelpers.Aggregates;
using EventFly.TestHelpers.Aggregates.Commands;
using EventFly.TestHelpers.Aggregates.Entities;
using EventFly.TestHelpers.Aggregates.Sagas.Test;
using EventFly.TestHelpers.Aggregates.Sagas.Test.Events;
using EventFly.TestHelpers.Aggregates.Sagas.TestAsync;
using EventFly.TestHelpers.Aggregates.Sagas.TestAsync.Events;
using EventFly.Tests.UnitTests.Subscribers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Tests.IntegrationTests.Aggregates.Sagas
{
    [Collection("AggregateSagaTests")]
    public class AggregateSagaTests : TestKit
    {
        private const string Category = "Sagas";
        public AggregateSagaTests(ITestOutputHelper testOutputHelper)
            : base(TestHelpers.Akka.Configuration.Config, "aggregate-saga-tests", testOutputHelper)
        {
            Sys.RegisterDependencyResolver(
                new ServiceCollection()
                .AddEventFly(
                    Sys,
                    ib => ib.AddSaga<TestSaga, TestSagaId>().AddSaga<TestAsyncSaga, TestAsyncSagaId>(),
                    db => db.RegisterDomainDefinitions<TestDomain>()
                )
                .Services
                    .AddScoped<TestSaga>()
                    .AddScoped<TestAsyncSaga>()
                .BuildServiceProvider()
            );
        }

        [Fact]
        [Category(Category)]
        public void SendingTest_FromTestAggregate_CompletesSaga()
        {
            var eventProbe = CreateTestProbe("event-probe");

            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaStartedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaCompletedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSaga, TestSagaId, TestSagaTransactionCompletedEvent>));
            //var aggregateManager = Sys.ActorOf(Props.Create(() => new AggregateManager<TestAggregate,TestAggregateId>()), "test-aggregatemanager");
            //Sys.ActorOf(Props.Create(() => new TestSagaManager(() => new TestSaga(aggregateManager))), "test-sagaaggregatemanager");
            var bus = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<ICommandBus>();



            var senderAggregateId = TestAggregateId.New;
            var senderCreateAggregateCommand = new CreateTestCommand(senderAggregateId, CommandId.New);
            bus.Publish(senderCreateAggregateCommand).GetAwaiter().GetResult();

            var receiverAggregateId = TestAggregateId.New;
            var receiverCreateAggregateCommand = new CreateTestCommand(receiverAggregateId, CommandId.New);
            bus.Publish(receiverCreateAggregateCommand).GetAwaiter().GetResult();

            var senderTestId = TestId.New;
            var senderTest = new Test(senderTestId);
            var nextAggregateCommand = new AddTestCommand(senderAggregateId, CommandId.New, senderTest);
            bus.Publish(nextAggregateCommand).GetAwaiter().GetResult();

            var sagaStartingCommand = new GiveTestCommand(senderAggregateId, CommandId.New, receiverAggregateId, senderTest);
            bus.Publish(sagaStartingCommand).GetAwaiter().GetResult();

            eventProbe.
                ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaStartedEvent>>(
                    x => x.AggregateEvent.Sender.Equals(senderAggregateId)
                         && x.AggregateEvent.Receiver.Equals(receiverAggregateId)
                         && x.AggregateEvent.SentTest.Equals(senderTest));

            eventProbe.
                ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaTransactionCompletedEvent>>();

            eventProbe.
                ExpectMsg<DomainEvent<TestSaga, TestSagaId, TestSagaCompletedEvent>>();
        }

        [Fact]
        [Category(Category)]
        public void SendingTest_FromTestAggregate_CompletesSagaAsync()
        {
            var eventProbe = CreateTestProbe("event-probe");

            var bus = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<ICommandBus>();

            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaStartedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaCompletedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaTransactionCompletedEvent>));
            //var aggregateManager = Sys.ActorOf(Props.Create(() => new AggregateManager<TestAggregate,TestAggregateId>()), "test-aggregatemanager");
            //Sys.ActorOf(Props.Create(() => new TestAsyncSagaManager(() => new TestAsyncSaga(aggregateManager))), "test-sagaaggregatemanager");

            var senderAggregateId = TestAggregateId.New;
            var senderCreateAggregateCommand = new CreateTestCommand(senderAggregateId, CommandId.New);
            bus.Publish(senderCreateAggregateCommand).GetAwaiter().GetResult();

            var receiverAggregateId = TestAggregateId.New;
            var receiverCreateAggregateCommand = new CreateTestCommand(receiverAggregateId, CommandId.New);
            bus.Publish(receiverCreateAggregateCommand).GetAwaiter().GetResult();

            var senderTestId = TestId.New;
            var senderTest = new Test(senderTestId);
            var nextAggregateCommand = new AddTestCommand(senderAggregateId, CommandId.New, senderTest);
            bus.Publish(nextAggregateCommand).GetAwaiter().GetResult();

            var sagaStartingCommand = new GiveTestCommand(senderAggregateId, CommandId.New, receiverAggregateId, senderTest);
            bus.Publish(sagaStartingCommand).GetAwaiter().GetResult();



            eventProbe.
                ExpectMsg<DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaStartedEvent>>(
                    x => x.AggregateEvent.Sender.Equals(senderAggregateId)
                         && x.AggregateEvent.Receiver.Equals(receiverAggregateId)
                         && x.AggregateEvent.SentTest.Equals(senderTest)
                         && x.Metadata.ContainsKey("some-key"));

            eventProbe.
                ExpectMsg<DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaTransactionCompletedEvent>>();

            eventProbe.
                ExpectMsg<DomainEvent<TestAsyncSaga, TestAsyncSagaId, TestAsyncSagaCompletedEvent>>();
        }
    }
}
