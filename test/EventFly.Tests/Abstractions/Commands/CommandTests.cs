using System;
using System.ComponentModel;
using EventFly.Commands;
using EventFly.Tests.Data.Abstractions;
using EventFly.Tests.Data.Abstractions.Commands;
using FluentAssertions;
using Xunit;

namespace EventFly.Tests.Abstractions.Commands
{
    [Category(Categories.Abstractions)]
    [Collection(Collections.Only)]
    public class CommandTests
    {
        [Fact]
        public void InstantiatingCommand_WithValidInput_ThrowsException()
        {
            var aggregateId = TestAggregateId.New;
            var sourceId = CommandId.New;

            var command = new CreateTestCommand(aggregateId, sourceId);

            command.Metadata.SourceId.Value.Should().Be(sourceId.Value);
        }

        [Fact]
        public void InstantiatingCommand_WithNullId_ThrowsException()
        {
            this.Invoking(test => new CreateTestCommand(null, CommandId.New))
                .Should().Throw<ArgumentNullException>().And.Message.Contains("aggregateId").Should().BeTrue();
        }
    }
}