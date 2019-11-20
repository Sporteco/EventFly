using EventFly.Commands;

namespace EventFly.Tests.Abstractions
{
    public class TestFailedExecutionResultCommand : Command<TestAggregateId>
    {
        public TestFailedExecutionResultCommand(TestAggregateId aggregateId, CommandId sourceId) : base(aggregateId, new CommandMetadata(sourceId)) { }
    }
}