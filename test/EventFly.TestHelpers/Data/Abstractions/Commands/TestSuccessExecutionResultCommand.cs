using EventFly.Commands;

namespace EventFly.Tests.Abstractions
{
    public class TestSuccessExecutionResultCommand : Command<TestAggregateId>
    {
        public TestSuccessExecutionResultCommand(TestAggregateId aggregateId, CommandId sourceId) : base(aggregateId, new CommandMetadata(sourceId)) { }
    }
}