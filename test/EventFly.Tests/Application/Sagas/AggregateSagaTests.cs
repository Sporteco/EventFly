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

using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.DependencyInjection;
using EventFly.TestFixture;
using EventFly.Tests.Abstractions;
using EventFly.Tests.Domain;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Tests.Application
{
    [Category(Categories.Application)]
    [Collection(Collections.Only)]
    public class AggregateSagaTests : AggregateTestKit<TestContext>
    {
        public AggregateSagaTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void SendingTest_FromTestAggregate_CompletesSaga()
        {
            var eventProbe = CreateTestProbe("event-probe");

            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSagaId, TestSagaStartedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSagaId, TestSagaCompletedEvent>));
            Sys.EventStream.Subscribe(eventProbe, typeof(DomainEvent<TestSagaId, TestSagaTransactionCompletedEvent>));
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

            eventProbe.ExpectMsg<DomainEvent<TestSagaId, TestSagaStartedEvent>>(
                x => x.AggregateEvent.Sender.Equals(senderAggregateId)
                && x.AggregateEvent.Receiver.Equals(receiverAggregateId)
                && x.AggregateEvent.SentTest.Equals(senderTest));

            eventProbe.ExpectMsg<DomainEvent<TestSagaId, TestSagaTransactionCompletedEvent>>();

            eventProbe.ExpectMsg<DomainEvent<TestSagaId, TestSagaCompletedEvent>>();
        }
    }
}