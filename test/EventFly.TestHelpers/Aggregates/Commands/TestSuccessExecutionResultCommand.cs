using EventFly.Commands;

namespace EventFly.TestHelpers.Aggregates.Commands
{
    public class TestSuccessExecutionResultCommand : Command<TestAggregateId>
    {
        public TestSuccessExecutionResultCommand(TestAggregateId aggregateId, CommandId sourceId) : base(aggregateId, new CommandMetadata(sourceId)) { }
    }
}