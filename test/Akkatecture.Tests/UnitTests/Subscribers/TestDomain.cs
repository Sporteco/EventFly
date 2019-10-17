using Akka.Actor;
using Akkatecture.Definitions;
using Akkatecture.TestHelpers.Aggregates;
using Akkatecture.TestHelpers.Aggregates.Commands;
using Akkatecture.TestHelpers.Aggregates.Events;
using Akkatecture.TestHelpers.Aggregates.Events.Errors;
using Akkatecture.TestHelpers.Aggregates.Events.Signals;
using Akkatecture.TestHelpers.Aggregates.Sagas.Test;
using Akkatecture.TestHelpers.Aggregates.Sagas.Test.Events;
using Akkatecture.TestHelpers.Aggregates.Sagas.TestAsync;
using Akkatecture.TestHelpers.Aggregates.Sagas.TestAsync.Events;
using Akkatecture.TestHelpers.Aggregates.Snapshots;
using Akkatecture.TestHelpers.Jobs;

namespace Akkatecture.Tests.UnitTests.Subscribers
{
    public class TestDomain : DomainDefinition
    {
        public TestDomain(ActorSystem system) : base()
        {
            RegisterAggregate<TestAggregate, TestAggregateId>();
            RegisterSaga<TestSaga, TestSagaId>();
            RegisterSaga<TestAsyncSaga, TestAsyncSagaId>();
            RegisterCommands(typeof(AddFourTestsCommand), typeof(AddTestCommand),typeof(CreateAndAddTwoTestsCommand),
                typeof(CreateTestCommand), typeof(GiveTestCommand), typeof(PoisonTestAggregateCommand), typeof(PublishTestStateCommand),
                typeof(ReceiveTestCommand), typeof(TestDomainErrorCommand), typeof(TestFailedExecutionResultCommand),
                typeof(PublishTestStateCommand));
            RegisterEvents(typeof(TestedErrorEvent), typeof(TestSagaStartedEvent), typeof(TestStateSignalEvent),
                typeof(TestAddedEvent), typeof(TestCreatedEvent), typeof(TestReceivedEvent),
                typeof(TestSentEvent), typeof(TestSagaCompletedEvent), typeof(TestSagaStartedEvent), typeof(TestSagaTransactionCompletedEvent),
                typeof(TestAsyncSagaCompletedEvent), typeof(TestAsyncSagaStartedEvent), typeof(TestAsyncSagaTransactionCompletedEvent));
            RegisterSnapshots(typeof(TestAggregateSnapshot));
            RegisterJobs(typeof(TestJob));

        }
    }
}