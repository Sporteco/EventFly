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
using System.Linq;
using Akka.Persistence;
using EventFly.Aggregates;
using EventFly.Aggregates.Snapshot;
using EventFly.Aggregates.Snapshot.Strategies;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using EventFly.Extensions;
using EventFly.Metadata;
using EventFly.TestHelpers.Aggregates.Commands;
using EventFly.TestHelpers.Aggregates.Events;
using EventFly.TestHelpers.Aggregates.Events.Errors;
using EventFly.TestHelpers.Aggregates.Events.Signals;
using EventFly.TestHelpers.Aggregates.Snapshots;

namespace EventFly.TestHelpers.Aggregates
{
    [AggregateName("Test")]
    public sealed class TestAggregate : EventSourcedAggregateRoot<TestAggregate, TestAggregateId, TestAggregateState>, 
        IExecute<CreateTestCommand,TestAggregateId>,
        IExecute<CreateAndAddTwoTestsCommand,TestAggregateId>,
        IExecute<AddTestCommand,TestAggregateId>,
        IExecute<AddFourTestsCommand,TestAggregateId>,
        IExecute<GiveTestCommand,TestAggregateId>,
        IExecute<ReceiveTestCommand,TestAggregateId>,
        IExecute<PoisonTestAggregateCommand,TestAggregateId>,
        IExecute<PublishTestStateCommand,TestAggregateId>,
        IExecute<TestDomainErrorCommand,TestAggregateId>,
        IExecute<TestFailedExecutionResultCommand,TestAggregateId>,
        IExecute<TestSuccessExecutionResultCommand,TestAggregateId>
    {
        public int TestErrors { get; private set; }
        public TestAggregate(TestAggregateId aggregateId)
            : base(aggregateId)
        {
            TestErrors = 0;

            Command<SaveSnapshotSuccess>(SnapshotStatus);

            SetSnapshotStrategy(new SnapshotEveryFewVersionsStrategy(10));
        }

        public IExecutionResult Execute(CreateTestCommand command)
        {
            if (IsNew)
            {
                Emit(new TestCreatedEvent(command.AggregateId), new EventMetadata {{"some-key","some-value"}});
                Reply(TestExecutionResult.SucceededWith(command.Metadata.SourceId));
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
                ReplyFailure(TestExecutionResult.FailedWith(command.Metadata.SourceId));
            }

            return new SuccessTestExecutionResult(command.Metadata.SourceId);
        }
        

        public IExecutionResult Execute(CreateAndAddTwoTestsCommand command)
        {
            if (IsNew)
            {
                var createdEvent = new TestCreatedEvent(command.AggregateId);
                var firstTestAddedEvent = new TestAddedEvent(command.FirstTest);
                var secondTestAddedEvent = new TestAddedEvent(command.SecondTest);
                EmitAll(createdEvent, firstTestAddedEvent, secondTestAddedEvent);
                Reply(TestExecutionResult.SucceededWith(command.Metadata.SourceId));
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
                ReplyFailure(TestExecutionResult.FailedWith(command.Metadata.SourceId));
            }

            return ExecutionResult.Success();
        }

        public IExecutionResult Execute(AddTestCommand command)
        {
            if (!IsNew)
            {

                Emit(new TestAddedEvent(command.Test));
                Reply(TestExecutionResult.SucceededWith(command.Metadata.SourceId));

            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
                ReplyFailure(TestExecutionResult.FailedWith(command.Metadata.SourceId));
            }
            return new SuccessTestExecutionResult(command.Metadata.SourceId);
        }

        public IExecutionResult Execute(AddFourTestsCommand command)
        {
            if (!IsNew)
            {
                var events = Enumerable
                    .Range(0, 4)
                    .Select(x => new TestAddedEvent(command.Test));

                // ReSharper disable once CoVariantArrayConversion
                EmitAll(events.ToArray());
                Reply(TestExecutionResult.SucceededWith(command.Metadata.SourceId));

            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
                ReplyFailure(TestExecutionResult.FailedWith(command.Metadata.SourceId));
            }
            return ExecutionResult.Success();
        }

        public IExecutionResult Execute(GiveTestCommand command)
        {
            if (!IsNew)
            {
                if (State.TestCollection.Any(x => x.Id == command.TestToGive.Id))
                {
                    Emit(new TestSentEvent(command.TestToGive,command.ReceiverAggregateId));
                    Reply(TestExecutionResult.SucceededWith(command.Metadata.SourceId));
                }

            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
                ReplyFailure(TestExecutionResult.FailedWith(command.Metadata.SourceId));
            }

            return ExecutionResult.Success();
        }

        public IExecutionResult Execute(ReceiveTestCommand command)
        {
            if (!IsNew)
            {
                Emit(new TestReceivedEvent(command.SenderAggregateId, command.TestToReceive));
                Reply(TestExecutionResult.SucceededWith(command.Metadata.SourceId));
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
                ReplyFailure(TestExecutionResult.FailedWith(command.Metadata.SourceId));
            }

            return ExecutionResult.Success();
        }
        public IExecutionResult Execute(TestFailedExecutionResultCommand command)
        {
            Sender.Tell(ExecutionResult.Failed(), Self);
            return ExecutionResult.Success();
        }
        
        public IExecutionResult Execute(TestSuccessExecutionResultCommand command)
        {
            Sender.Tell(ExecutionResult.Success(), Self);
            return ExecutionResult.Success();
        }

        public IExecutionResult Execute(PoisonTestAggregateCommand command)
        {
            if (!IsNew)
            {
                Context.Stop(Self);
            }
            else
            {
                TestErrors++;
                Throw(new TestedErrorEvent(TestErrors));
                ReplyFailure(TestExecutionResult.FailedWith(command.Metadata.SourceId));
            }

            return ExecutionResult.Success();
        }

        public IExecutionResult Execute(PublishTestStateCommand command)
        {
            Signal(new TestStateSignalEvent(State,LastSequenceNr,Version));

            return ExecutionResult.Success();
        }


        public IExecutionResult Execute(TestDomainErrorCommand command)
        {
            TestErrors++;
            Throw(new TestedErrorEvent(TestErrors));

            return ExecutionResult.Success();
        }

        protected override bool SnapshotStatus(SaveSnapshotSuccess snapshotSuccess)
        {
            Context.Stop(Self);
            Context.Parent.Tell(new PublishTestStateCommand(Id), Self);
            return true;
        }

        protected override IAggregateSnapshot<TestAggregate, TestAggregateId> CreateSnapshot()
        {
            return new TestAggregateSnapshot(State.TestCollection
                .Select(x => new TestAggregateSnapshot.TestModel(x.Id.GetGuid())).ToList());
        }

        private void Signal<TAggregateEvent>(TAggregateEvent aggregateEvent, IEventMetadata metadata = null)
            where TAggregateEvent : class, IAggregateEvent<TestAggregateId>
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }

            var aggregateSequenceNumber = Version;
            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{Id.Value}-v{aggregateSequenceNumber}");
            var now = DateTimeOffset.UtcNow;
            var eventMetadata = new EventMetadata
            {
                Timestamp = now,
                AggregateSequenceNumber = aggregateSequenceNumber,
                AggregateName = Name.Value,
                AggregateId = Id.Value,
                EventId = eventId
            };

            eventMetadata.Add(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }

            var domainEvent = new DomainEvent<TestAggregate, TestAggregateId, TAggregateEvent>(Id, aggregateEvent, eventMetadata, now, Version);

            Publish(domainEvent);
        }

        private void Throw<TAggregateEvent>(TAggregateEvent aggregateEvent, IEventMetadata metadata = null)
            where TAggregateEvent : class, IAggregateEvent<TestAggregateId>
        {
            Signal(aggregateEvent, metadata);
        }
    }
}