using EventFly.Commands;

namespace EventFly.TestHelpers.Aggregates.Commands
{
    public class TestFailedExecutionResultCommand : Command<TestAggregateId>
    {
        public TestFailedExecutionResultCommand(
            TestAggregateId aggregateId,
            CommandId sourceId)
            : base(aggregateId, new CommandMetadata(sourceId))
        {
        }
    }
}