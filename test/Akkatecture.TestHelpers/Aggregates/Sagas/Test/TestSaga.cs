// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
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
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Sagas.Test.Events;

namespace Akkatecture.TestHelpers.Aggregates.Sagas.Test
{
    public class TestSaga : AggregateSaga<TestSaga,TestSagaId,TestSagaState>,
        ISagaIsStartedBy<TestAggregateId, TestSentEvent>,
        ISagaHandles<TestAggregateId, TestReceivedEvent>
    {
        public TestSaga()
        {
            
            Command<EmitTestSagaState>(Handle);
        }

        public bool Handle(IDomainEvent<TestAggregateId, TestSentEvent> domainEvent)
        {
            if (IsNew)
            {
                var command = new ReceiveTestCommand(
                    domainEvent.AggregateEvent.RecipientAggregateId,
                    CommandId.New,
                    domainEvent.AggregateIdentity,
                    domainEvent.AggregateEvent.Test);

                Emit(new TestSagaStartedEvent(domainEvent.AggregateIdentity, domainEvent.AggregateEvent.RecipientAggregateId, domainEvent.AggregateEvent.Test));

                Context.System.PublishCommandAsync(command).GetAwaiter().GetResult();
            }

            return true;
        }

        public bool Handle(IDomainEvent<TestAggregateId, TestReceivedEvent> domainEvent)
        {
            if (!IsNew)
            {
                Emit(new TestSagaTransactionCompletedEvent());
                Self.Tell(new EmitTestSagaState());

            }
            return true;
        }

        private bool Handle(EmitTestSagaState testCommand)
        {
            Emit(new TestSagaCompletedEvent(State));
            return true;
        }

        private class EmitTestSagaState
        {
        }
    }
    
    
}
