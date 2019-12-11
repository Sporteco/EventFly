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

using System;
using System.ComponentModel;
using System.Linq;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.Core;
using EventFly.Extensions;
using EventFly.Tests.Data.Abstractions;
using EventFly.Tests.Data.Abstractions.Commands;
using EventFly.Tests.Data.Abstractions.Entities;
using EventFly.Tests.Data.Abstractions.Events;
using EventFly.Tests.Data.Abstractions.Events.Upcasters;
using EventFly.Tests.Data.Domain;
using FluentAssertions;
using Xunit;

namespace EventFly.Tests.Domain.Mapping
{
    [Category(Categories.Domain)]
    [Collection(Collections.Only)]
    public class AggregateEventUpcasterTests
    {
        [Fact]
        public void CommittedEvent_WithUpgradableEvent_GetsUpgrades()
        {
            var aggregateEventUpcaster = new TestAggregateEventUpcaster();
            var aggregateSequenceNumber = 3;
            var aggregateId = TestAggregateId.New;
            var aggregateEvent = new TestCreatedEvent(aggregateId);
            var now = DateTimeOffset.UtcNow;
            var eventId = EventId.NewDeterministic(GuidFactories.Deterministic.Namespaces.Events, $"{aggregateId.Value}-v{3}");
            var eventMetadata = new EventMetadata
            {
                Timestamp = now,
                AggregateSequenceNumber = aggregateSequenceNumber,
                AggregateName = typeof(TestAggregate).GetAggregateName().Value,
                AggregateId = aggregateId.Value,
                EventId = eventId
            };
            var committedEvent = new CommittedEvent<TestAggregateId, TestCreatedEvent>(
                aggregateId,
                aggregateEvent,
                eventMetadata,
                now,
                aggregateSequenceNumber);

            var eventSequence = aggregateEventUpcaster.FromJournal(committedEvent, String.Empty);
            var upcastedEvent = eventSequence.Events.Single();

            upcastedEvent
                .As<ICommittedEvent<TestAggregateId, TestCreatedEventV2>>()
                .AggregateEvent.Name
                .Should().Be("default upcasted string");
        }

        [Fact]
        public void Upcasting_UnsupportedEvent_ThrowsException()
        {
            var aggregateEventUpcaster = new TestAggregateEventUpcaster();
            var aggregateEvent = new TestAddedEvent(Test.New);

            this.Invoking(test => aggregateEventUpcaster.Upcast(aggregateEvent))
                .Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Instantiating_UnInstantiableUpcaster_ThrowsException()
        {
            this.Invoking(test => new UnInstantiableAggregateEventUpcaster())
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void NonCommittedEvent_WhenRead_IsReturnedUnchanged()
        {
            var message = new CreateTestCommand(TestAggregateId.New, CommandId.New);
            var eventUpcaster = new TestAggregateEventUpcaster();

            var unchanged = eventUpcaster.FromJournal(message, String.Empty);

            unchanged.Events.Single().As<CreateTestCommand>().Metadata.SourceId.Should().Be(message.Metadata.SourceId);
            unchanged.Events.Single().As<CreateTestCommand>().AggregateId.Should().Be(message.AggregateId);
        }
    }
}