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

using Akka.Actor;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.Sagas;
using EventFly.Sagas.AggregateSaga;
using EventFly.Tests.Abstractions;
using System;
using System.Threading.Tasks;

namespace EventFly.Tests.Application
{
    public class TestSaga : AggregateSaga<TestSaga, TestSagaId, TestSagaState>,
        ISagaIsStartedBy<TestAggregateId, TestSentEvent>,
        ISagaHandles<TestAggregateId, TestReceivedEvent>
    {
        public TestSaga()
        {
            Command<EmitTestSagaState>(Handle);
        }

        public async Task Handle(IDomainEvent<TestAggregateId, TestSentEvent> domainEvent)
        {
            if (IsNew)
            {
                var command = new ReceiveTestCommand(
                    domainEvent.AggregateEvent.RecipientAggregateId,
                    CommandId.New,
                    domainEvent.AggregateIdentity,
                    domainEvent.AggregateEvent.Test);

                Emit(new TestSagaStartedEvent(domainEvent.AggregateIdentity, domainEvent.AggregateEvent.RecipientAggregateId, domainEvent.AggregateEvent.Test));

                await PublishCommandAsync(command);
            }
        }

        public Task Handle(IDomainEvent<TestAggregateId, TestReceivedEvent> domainEvent)
        {
            if (!IsNew)
            {
                Emit(new TestSagaTransactionCompletedEvent());
                Self.Tell(new EmitTestSagaState());
            }

            return Task.CompletedTask;
        }

        private Boolean Handle(EmitTestSagaState testCommand)
        {
            Emit(new TestSagaCompletedEvent(State));
            return true;
        }

        private class EmitTestSagaState { }
    }
}