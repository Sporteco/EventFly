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

using EventFly.Aggregates;
using EventFly.Aggregates.Snapshot;
using EventFly.Sagas;
using EventFly.TestHelpers.Aggregates.Entities;
using EventFly.TestHelpers.Aggregates.Sagas.TestAsync.Events;
using System.Threading.Tasks;

namespace EventFly.TestHelpers.Aggregates.Sagas.TestAsync
{
    public class TestAsyncSagaState : SagaState<TestAsyncSaga, TestAsyncSagaId, IMessageApplier<TestAsyncSaga, TestAsyncSagaId>>,
        IApply<TestAsyncSagaStartedEvent>,
        IApply<TestAsyncSagaTransactionCompletedEvent>,
        IApply<TestAsyncSagaCompletedEvent>,
        IHydrate<TestAsyncSagaSnapshot>
    {
        public TestAggregateId Sender { get; set; }
        public TestAggregateId Receiver { get; set; }
        public Entities.Test Test { get; set; }
        public async Task Apply(TestAsyncSagaStartedEvent aggregateEvent)
        {
            Sender = aggregateEvent.Sender;
            Receiver = aggregateEvent.Receiver;
            Test = aggregateEvent.SentTest;
        }

        public async Task Apply(TestAsyncSagaTransactionCompletedEvent aggregateEvent)
        {
        }

        public async Task Apply(TestAsyncSagaCompletedEvent aggregateEvent)
        {
        }

        public void Hydrate(TestAsyncSagaSnapshot aggregateSnapshot)
        {
            Sender = TestAggregateId.With(aggregateSnapshot.SenderId);
            Receiver = TestAggregateId.With(aggregateSnapshot.ReceiverId);
            Test = new Entities.Test(TestId.With(aggregateSnapshot.Test.Id));
        }
    }
}