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
using Akka.Actor;
using Akka.Persistence;
using Akka.TestKit;
using EventFly.Aggregates;
using EventFly.Aggregates.Snapshot;
using EventFly.Commands;
using EventFly.Core;
using EventFly.DependencyInjection;
using EventFly.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using AkkaSnapshotMetadata = Akka.Persistence.SnapshotMetadata;
using SnapshotMetadata = EventFly.Aggregates.Snapshot.SnapshotMetadata;

namespace EventFly.TestFixture.Aggregates
{
    public class AggregateFixture<TAggregate, TIdentity> : IFixtureArranger<TAggregate, TIdentity>, IFixtureExecutor<TAggregate, TIdentity>, IFixtureAsserter<TAggregate, TIdentity>
        where TAggregate : ActorBase, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        private readonly TestKitBase _testKit;
        public TIdentity AggregateId { get; private set; }
        public TestProbe AggregateEventTestProbe { get; private set; }
        public TestProbe AggregateReplyTestProbe { get; private set; }
        public AggregateFixture(
            TestKitBase testKit)
        {
            _testKit = testKit;
        }
        
        private ICommandBus _commandBus;
        public ICommandBus CommandBus
        {
            get
            {
                if (_commandBus == null)
                {
                    _commandBus =  _testKit.Sys.GetExtension<ServiceProviderHolder>()?.ServiceProvider?.GetService<ICommandBus>();
                    if (_commandBus != null && _commandBus is TestCommandBus tcb)
                    {
                        tcb.SetSender(AggregateReplyTestProbe);
                    }
                }

                return _commandBus;
            }
        }


        public IFixtureArranger<TAggregate, TIdentity> For(TIdentity aggregateId)
        {
            if (aggregateId == null)
                throw new ArgumentNullException(nameof(aggregateId));

            if (!AggregateEventTestProbe.IsNobody())
                throw new InvalidOperationException(nameof(AggregateEventTestProbe));

            AggregateId = aggregateId;
            AggregateEventTestProbe = _testKit.CreateTestProbe("aggregate-event-test-probe");
            AggregateReplyTestProbe = _testKit.CreateTestProbe("aggregate-reply-test-probe");

            return this;
        }

        public IFixtureArranger<TAggregate, TIdentity> Using(TIdentity aggregateId)
        {
            if (aggregateId == null)
                throw new ArgumentNullException(nameof(aggregateId));
            if (!AggregateEventTestProbe.IsNobody())
                throw new InvalidOperationException(nameof(AggregateEventTestProbe));
            if (!AggregateReplyTestProbe.IsNobody())
                throw new InvalidOperationException(nameof(AggregateReplyTestProbe));

            AggregateId = aggregateId;
            AggregateEventTestProbe = _testKit.CreateTestProbe("aggregate-event-test-probe");
            AggregateReplyTestProbe = _testKit.CreateTestProbe("aggregate-reply-test-probe");

            return this;
        }

        public IFixtureExecutor<TAggregate, TIdentity> GivenNothing()
        {
            return this;
        }

        public IFixtureExecutor<TAggregate, TIdentity> Given(params IAggregateEvent<TIdentity>[] aggregateEvents)
        {
            InitializeEventJournal(AggregateId, aggregateEvents);

            return this;
        }

        public IFixtureExecutor<TAggregate, TIdentity> Given(IAggregateSnapshot<TAggregate, TIdentity> aggregateSnapshot, long snapshotSequenceNumber)
        {
            InitializeSnapshotStore(AggregateId, aggregateSnapshot, snapshotSequenceNumber);

            return this;
        }

        public IFixtureExecutor<TAggregate, TIdentity> Given(params ICommand[] commands)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));
            if (CommandBus is TestCommandBus tcb)
            {
                tcb.SetSender(null);
            }

            foreach (var command in commands)
            {
                if (command == null)
                    throw new NullReferenceException(nameof(command));

                var result = CommandBus.Publish(command).GetAwaiter().GetResult();
                if (!result.IsSuccess)
                    throw new InvalidOperationException($"Given Command {command.GetType().PrettyPrint()} failed.");

            }
            
            if (CommandBus is TestCommandBus tcb2)
            {
                tcb2.SetSender(AggregateReplyTestProbe);
            }

            return this;
        }

        public IFixtureAsserter<TAggregate, TIdentity> When(params ICommand[] commands)
        {

            if (commands == null)
                throw new ArgumentNullException(nameof(commands));

            foreach (var command in commands)
            {
                if (command == null)
                    throw new NullReferenceException(nameof(command));

                CommandBus.Publish(command).GetAwaiter().GetResult();
                //do nothing because we receive result via AggregateReplyTestProbe
            }

            return this;
        }

        public IFixtureAsserter<TAggregate, TIdentity> AndWhen(params ICommand[] commands)
        {
            return When(commands);
        }

        public IFixtureAsserter<TAggregate, TIdentity> ThenExpect<TAggregateEvent>(Predicate<TAggregateEvent> aggregateEventPredicate = null)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            _testKit.Sys.EventStream.Subscribe(AggregateEventTestProbe, typeof(IDomainEvent<TIdentity, TAggregateEvent>));

            if (aggregateEventPredicate == null)
                AggregateEventTestProbe.ExpectMsg<DomainEvent<TAggregate, TIdentity, TAggregateEvent>>();
            else
                AggregateEventTestProbe.ExpectMsg<DomainEvent<TAggregate, TIdentity, TAggregateEvent>>(x => aggregateEventPredicate(x.AggregateEvent));

            return this;
        }

        public IFixtureAsserter<TAggregate, TIdentity> ThenExpectReply<TReply>(Predicate<TReply> aggregateReplyPredicate = null)
        {
            AggregateReplyTestProbe.ExpectMsg(aggregateReplyPredicate);
            return this;
        }

        public IFixtureAsserter<TAggregate, TIdentity> ThenExpectDomainEvent<TAggregateEvent>(Predicate<IDomainEvent<TIdentity, TAggregateEvent>> domainEventPredicate = null)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            _testKit.Sys.EventStream.Subscribe(AggregateEventTestProbe, typeof(IDomainEvent<TIdentity, TAggregateEvent>));

            if (domainEventPredicate == null)
                AggregateEventTestProbe.ExpectMsg<DomainEvent<TAggregate, TIdentity, TAggregateEvent>>();
            else
                AggregateEventTestProbe.ExpectMsg<DomainEvent<TAggregate, TIdentity, TAggregateEvent>>(domainEventPredicate);

            return this;
        }

        private void InitializeEventJournal(TIdentity aggregateId, params IAggregateEvent<TIdentity>[] events)
        {
            var writerGuid = Guid.NewGuid().ToString();
            var writes = new AtomicWrite[events.Length];
            for (var i = 0; i < events.Length; i++)
            {
                var committedEvent = new CommittedEvent<TAggregate, TIdentity, IAggregateEvent<TIdentity>>(aggregateId, events[i], new EventMetadata(), DateTimeOffset.UtcNow, i + 1);
                writes[i] = new AtomicWrite(new Persistent(committedEvent, i + 1, aggregateId.Value, string.Empty, false, ActorRefs.NoSender, writerGuid));
            }
            var journal = Persistence.Instance.Apply(_testKit.Sys).JournalFor(null);
            journal.Tell(new WriteMessages(writes, AggregateEventTestProbe.Ref, 1));

            AggregateEventTestProbe.ExpectMsg<WriteMessagesSuccessful>();

            for (var i = 0; i < events.Length; i++)
            {
                var seq = i;
                AggregateEventTestProbe.ExpectMsg<WriteMessageSuccess>(x =>
                    x.Persistent.PersistenceId == aggregateId.ToString() &&
                    x.Persistent.Payload is CommittedEvent<TAggregate, TIdentity, IAggregateEvent<TIdentity>> &&
                    x.Persistent.SequenceNr == (long)seq + 1);
            }
        }

        private void InitializeSnapshotStore<TAggregateSnapshot>(TIdentity aggregateId, TAggregateSnapshot aggregateSnapshot, long sequenceNumber)
            where TAggregateSnapshot : IAggregateSnapshot<TAggregate, TIdentity>
        {
            var snapshotStore = Persistence.Instance.Apply(_testKit.Sys).SnapshotStoreFor(null);
            var committedSnapshot = new CommittedSnapshot<TAggregate, TIdentity, TAggregateSnapshot>(aggregateId, aggregateSnapshot, new SnapshotMetadata(), DateTimeOffset.UtcNow, sequenceNumber);

            var metadata = new AkkaSnapshotMetadata(aggregateId.ToString(), sequenceNumber);
            snapshotStore.Tell(new SaveSnapshot(metadata, committedSnapshot), AggregateEventTestProbe.Ref);

            AggregateEventTestProbe.ExpectMsg<SaveSnapshotSuccess>(x =>
                x.Metadata.SequenceNr == sequenceNumber &&
                x.Metadata.PersistenceId == aggregateId.ToString());
        }
    }
}