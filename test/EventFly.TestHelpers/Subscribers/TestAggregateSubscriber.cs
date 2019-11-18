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
using EventFly.Subscribers;
using EventFly.TestHelpers.Aggregates;
using EventFly.TestHelpers.Aggregates.Events;

namespace EventFly.TestHelpers.Subscribers
{
    public class TestAggregateSubscriber : DomainEventSubscriber,
        ISubscribeTo<TestAggregateId, TestCreatedEvent>,
        ISubscribeTo<TestAggregateId, TestAddedEvent>
    {
        public System.Boolean Handle(IDomainEvent<TestAggregateId, TestCreatedEvent> domainEvent)
        {
            var handled = new TestSubscribedEventHandled<TestCreatedEvent>(domainEvent.AggregateEvent);
            Context.System.EventStream.Publish(handled);
            return true;
        }

        public System.Boolean Handle(IDomainEvent<TestAggregateId, TestAddedEvent> domainEvent)
        {
            var handled = new TestSubscribedEventHandled<TestAddedEvent>(domainEvent.AggregateEvent);
            Context.System.EventStream.Publish(handled);

            return true;
        }
    }


    public class TestSubscribedEventHandled<TAggregateEvent>
    {
        public TAggregateEvent AggregateEvent { get; }

        public TestSubscribedEventHandled(TAggregateEvent aggregateEvent)
        {
            AggregateEvent = aggregateEvent;
        }
    }
}