using EventFly.Definitions;
using EventFly.Tests.Abstractions;
using EventFly.Tests.Application;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.Tests.Domain
{
    public class TestContext : ContextDefinition
    {
        public TestContext()
        {
            RegisterAggregate<TestAggregate, TestAggregateId>();

            RegisterCommands(
                typeof(AddFourTestsCommand),
                typeof(AddTestCommand),
                typeof(CreateAndAddTwoTestsCommand),
                typeof(CreateTestCommand),
                typeof(GiveTestCommand),
                typeof(PoisonTestAggregateCommand),
                typeof(PublishTestStateCommand),
                typeof(ReceiveTestCommand),
                typeof(TestDomainErrorCommand),
                typeof(TestFailedExecutionResultCommand),
                typeof(PublishTestStateCommand));

            RegisterPublicEvents(
                typeof(TestedErrorEvent),
                typeof(TestSagaStartedEvent),
                typeof(TestStateSignalEvent),
                typeof(TestAddedEvent),
                typeof(TestCreatedEvent),
                typeof(TestReceivedEvent),
                typeof(TestSentEvent),
                typeof(TestSagaCompletedEvent),
                typeof(TestSagaStartedEvent),
                typeof(TestSagaTransactionCompletedEvent)
                );

            RegisterSnapshots(typeof(TestAggregateSnapshot));

            //RegisterJob<TestJob, TestJobId, TestJobRunner, TestJobScheduler>();
            //RegisterJob<AsyncTestJob, AsyncTestJobId, AsyncTestJobRunner, AsyncTestJobScheduler>();

            RegisterSaga<TestSaga, TestSagaId>();
        }

        public override IServiceCollection DI(IServiceCollection serviceDescriptors) => serviceDescriptors;
    }
}