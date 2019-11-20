using Akka.Actor;
using Akka.Persistence;
using Akka.TestKit.Xunit2;
using EventFly.Aggregates;
using EventFly.Aggregates.Snapshot;
using EventFly.DependencyInjection;
using EventFly.TestFixture;
using EventFly.Tests.Abstractions;
using EventFly.Tests.Application;
using EventFly.Tests.Domain;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace EventFly.Tests.TestFixture
{
    [Category(Categories.TestFixture)]
    [Collection("FixtureTests")]
    public class AggregateFixtureTests
    {
        [Fact]
        public void FixtureArrangerWithIdentity_CreatesAggregateRef()
        {
            //TODO - https://dev.azure.com/lutando/EventFly/_workitems/edit/23/
            using var testKit = new TestKit(Configuration.Default, "fixture-tests-1");
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(testKit);
            var aggregateIdentity = TestAggregateId.New;

            fixture
                .For(aggregateIdentity)
                .GivenNothing();

            fixture.AggregateId.Should().Be(aggregateIdentity);
        }

        [Fact]
        public void FixtureArrangerWithAggregateManager_CreatesAggregateManagerRef()
        {
            using var testKit = new TestKit(Configuration.Default, "fixture-tests-2");
            testKit.Sys.RegisterDependencyResolver(
                new ServiceCollection()
                .AddEventFly(testKit.Sys)
                .WithContext<TestContext>()
                .Services
                .AddScoped<TestSaga>()
                .BuildServiceProvider()
                .UseEventFly()
                );

            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(testKit);
            var aggregateIdentity = TestAggregateId.New;

            fixture.Using(aggregateIdentity);

            fixture.AggregateId.Should().Be(aggregateIdentity);
        }

        [Fact]
        public void FixtureArrangerWithEvents_CanBeReplayed()
        {
            using var testKit = new TestKit(Configuration.Default, "fixture-tests-3");
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(testKit);
            var aggregateIdentity = TestAggregateId.New;
            var events = new List<IAggregateEvent<TestAggregateId>>();
            events.Add(new TestCreatedEvent(aggregateIdentity));
            events.AddRange(Enumerable.Range(0, 10).Select(x => new TestAddedEvent(new Test(TestId.New))));
            var journal = Persistence.Instance.Apply(testKit.Sys).JournalFor(null);
            var receiverProbe = testKit.CreateTestProbe("journal-probe");
            fixture
                .For(aggregateIdentity)
                .Given(events.ToArray());

            journal.Tell(new ReplayMessages(1, Int64.MaxValue, Int64.MaxValue, aggregateIdentity.ToString(), receiverProbe.Ref));

            var from = 1;
            foreach (var _ in events)
            {
                var index = from;
                receiverProbe.ExpectMsg<ReplayedMessage>(x =>
                    x.Persistent.SequenceNr == index &&
                    x.Persistent.Payload is ICommittedEvent<TestAggregateId, IAggregateEvent<TestAggregateId>>);
                from++;
            }

            receiverProbe.ExpectMsg<RecoverySuccess>();
        }

        [Fact]
        public void FixtureArrangerWithSnapshot_CanBeLoaded()
        {
            using var testKit = new TestKit(Configuration.Default, "fixture-tests-4");
            var fixture = new AggregateFixture<TestAggregate, TestAggregateId>(testKit);
            var aggregateIdentity = TestAggregateId.New;
            var snapshot = new TestAggregateSnapshot(Enumerable.Range(0, 10).Select(x => new TestAggregateSnapshot.TestModel(Guid.NewGuid())).ToList());
            var snapshotStore = Persistence.Instance.Apply(testKit.Sys).SnapshotStoreFor(null);
            var receiverProbe = testKit.CreateTestProbe("snapshot-probe");
            var snapshotSequenceNumber = 1L;
            fixture
                .For(aggregateIdentity)
                .Given(snapshot, snapshotSequenceNumber);

            snapshotStore.Tell(new LoadSnapshot(aggregateIdentity.Value, new SnapshotSelectionCriteria(Int64.MaxValue, DateTime.MaxValue), Int64.MaxValue), receiverProbe.Ref);

            receiverProbe.ExpectMsg<LoadSnapshotResult>(x =>
                x.Snapshot.Snapshot is CommittedSnapshot<TestAggregate, TestAggregateId, IAggregateSnapshot<TestAggregate, TestAggregateId>> &&
                x.Snapshot.Metadata.SequenceNr == snapshotSequenceNumber &&
                x.Snapshot.Metadata.PersistenceId == aggregateIdentity.Value &&
                x.Snapshot.Snapshot
                    .As<CommittedSnapshot<TestAggregate, TestAggregateId, IAggregateSnapshot<TestAggregate, TestAggregateId>>>().AggregateSnapshot
                    .As<TestAggregateSnapshot>().Tests.Count == snapshot.Tests.Count &&
                x.ToSequenceNr == Int64.MaxValue);
        }
    }
}