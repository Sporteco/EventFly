using EventFly.Commands;

namespace EventFly.Tests.Data.Abstractions.Commands
{
    public class TestSuccessExecutionResultCommand : Command<TestAggregateId>
    {
        public TestSuccessExecutionResultCommand(TestAggregateId aggregateId, CommandId sourceId) : base(aggregateId, new CommandMetadata(sourceId)) { }
    }
}