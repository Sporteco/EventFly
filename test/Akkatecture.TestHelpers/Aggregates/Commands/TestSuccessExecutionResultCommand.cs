using Akkatecture.Commands;

namespace Akkatecture.TestHelpers.Aggregates.Commands
{
    public class TestSuccessExecutionResultCommand : Command<TestAggregateId>
    {
        public TestSuccessExecutionResultCommand(
            TestAggregateId aggregateId,
            CommandId sourceId)
            : base(aggregateId,  new CommandMetadata(sourceId))
        {
        }
    }
}