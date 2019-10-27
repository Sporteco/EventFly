using EventFly.Definitions;
using EventFly.TestHelpers.Aggregates;
using EventFly.TestHelpers.Aggregates.Commands;
using EventFly.TestHelpers.Aggregates.Events;
using EventFly.TestHelpers.Aggregates.Events.Errors;
using EventFly.TestHelpers.Aggregates.Events.Signals;
using EventFly.TestHelpers.Aggregates.Sagas.Test;
using EventFly.TestHelpers.Aggregates.Sagas.Test.Events;
using EventFly.TestHelpers.Aggregates.Sagas.TestAsync;
using EventFly.TestHelpers.Aggregates.Sagas.TestAsync.Events;
using EventFly.TestHelpers.Aggregates.Snapshots;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.Tests.UnitTests.Subscribers
{
    public class TestContext : ContextDefinition
    {
        public TestContext()
        {
            RegisterAggregate<TestAggregate, TestAggregateId>();

            RegisterCommands(typeof(AddFourTestsCommand), typeof(AddTestCommand),typeof(CreateAndAddTwoTestsCommand),
                typeof(CreateTestCommand), typeof(GiveTestCommand), typeof(PoisonTestAggregateCommand), typeof(PublishTestStateCommand),
                typeof(ReceiveTestCommand), typeof(TestDomainErrorCommand), typeof(TestFailedExecutionResultCommand),
                typeof(PublishTestStateCommand));
            RegisterEvents(typeof(TestedErrorEvent), typeof(TestSagaStartedEvent), typeof(TestStateSignalEvent),
                typeof(TestAddedEvent), typeof(TestCreatedEvent), typeof(TestReceivedEvent),
                typeof(TestSentEvent), typeof(TestSagaCompletedEvent), typeof(TestSagaStartedEvent), typeof(TestSagaTransactionCompletedEvent),
                typeof(TestAsyncSagaCompletedEvent), typeof(TestAsyncSagaStartedEvent), typeof(TestAsyncSagaTransactionCompletedEvent));
            RegisterSnapshots(typeof(TestAggregateSnapshot));
            RegisterSnapshots(typeof(TestAsyncSagaSnapshot));
            //RegisterJob<TestJob, TestJobId, TestJobRunner, TestJobScheduler>();
            //RegisterJob<AsyncTestJob, AsyncTestJobId, AsyncTestJobRunner, AsyncTestJobScheduler>();
            RegisterSaga<TestSaga, TestSagaId>();
            RegisterSaga<TestAsyncSaga, TestAsyncSagaId>();
        }

        public override IServiceCollection DI(IServiceCollection serviceDescriptors)
        {
            return serviceDescriptors;
        }
    }
}