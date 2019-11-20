using EventFly.Commands;

namespace EventFly.Tests.Abstractions
{
    public class CreateAndAddTwoTestsCommand : Command<TestAggregateId>
    {
        public Test FirstTest { get; }
        public Test SecondTest { get; }

        public CreateAndAddTwoTestsCommand(TestAggregateId aggregateId, CommandId sourceId, Test firstTest, Test secondTest)
            : base(aggregateId, new CommandMetadata(sourceId))
        {
            FirstTest = firstTest;
            SecondTest = secondTest;
        }
    }
}