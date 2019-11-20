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

using Akka.Persistence.Journal;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.Core;
using EventFly.Definitions;
using EventFly.Events;
using EventFly.Extensions;
using EventFly.Tests.Abstractions;
using FluentAssertions;
using System;
using System.ComponentModel;
using Xunit;

namespace EventFly.Tests.Domain
{
    [Category(Categories.Domain)]
    [Collection(Collections.Only)]
    public class AggregateEventTaggerTests
    {
        [Fact]
        public void CommittedEvent_WhenTagged_ContainsAggregateNameAsTaggedElement()
        {
            var aggregateEventTagger = new AggregateEventTagger(new EventDefinitions());
            var aggregateSequenceNumber = 3;
            var aggregateId = TestAggregateId.New;
            var entityId = TestId.New;
            var entity = new Test(entityId);
            var aggregateEvent = new TestAddedEvent(entity);
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
            var committedEvent = new CommittedEvent<TestAggregateId, TestAddedEvent>(
                aggregateId,
                aggregateEvent,
                eventMetadata,
                now,
                aggregateSequenceNumber);

            var taggedEvent = aggregateEventTagger.ToJournal(committedEvent);

            if (taggedEvent is Tagged a) a.Tags.Should().Contain(typeof(TestAggregateId).GetAggregateName().Value);
            else false.Should().BeTrue();
        }

        [Fact]
        public void CommittedEvent_WhenTagged_ContainsAggregateEventAsTaggedElement()
        {
            var aggregateEventTagger = new AggregateEventTagger(new EventDefinitions());
            var aggregateSequenceNumber = 3;
            var aggregateId = TestAggregateId.New;
            var entityId = TestId.New;
            var entity = new Test(entityId);
            var aggregateEvent = new TestAddedEvent(entity);
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
            var committedEvent = new CommittedEvent<TestAggregateId, TestAddedEvent>(
                aggregateId,
                aggregateEvent,
                eventMetadata,
                now,
                aggregateSequenceNumber);

            var taggedEvent = aggregateEventTagger.ToJournal(committedEvent);

            if (taggedEvent is Tagged a) a.Tags.Should().Contain("TestAdded");
            else false.Should().BeTrue();
        }

        [Fact]
        public void AggregateEventTagger_Manifest_ShouldBeEmpty()
        {
            var aggregateEventTagger = new AggregateEventTagger(new EventDefinitions());

            aggregateEventTagger.Manifest(null).Should().Be(String.Empty);
        }

        [Fact]
        public void AggregateEventTagger_TaggingNonEvent_ShouldBeLeftAlone()
        {
            var aggregateEventTagger = new AggregateEventTagger(new EventDefinitions());
            var command = new CreateTestCommand(TestAggregateId.New, CommandId.New);

            var untagged = aggregateEventTagger.ToJournal(command);

            command.Should().Be(untagged);
        }
    }
}