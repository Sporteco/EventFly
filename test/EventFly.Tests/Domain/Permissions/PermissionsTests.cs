using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.Metadata;
using EventFly.TestFixture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Tests.Domain
{
    [Category(Categories.Domain)]
    [Collection("PermissionsTests")]
    public class PermissionsTests : AggregateTestKit<TestPermissionsContext>
    {
        public PermissionsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void CommandAuthorizedAttributeTest()
        {
            var aggregateId = TestPermissionsId.New;
            FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestPermissionsAuthorizedAttributeCommand(aggregateId))
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }

        [Fact]
        public void CommandAuthorizedAttributeWithUserTest()
        {
            var aggregateId = TestPermissionsId.New;
            FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestPermissionsAuthorizedAttributeCommand(aggregateId, Metadata))
                .ThenExpectReply<SuccessExecutionResult>(x => x.IsSuccess);
        }

        [Fact]
        public void CommandAuthorizedInAggregateTest()
        {
            var aggregateId = TestPermissionsId.New;
            FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestPermissionsAuthorizedInAggregateCommand(aggregateId))
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }

        [Fact]
        public void CommandAuthorizedInAggregateWithUserTest()
        {
            var aggregateId = TestPermissionsId.New;
            FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestPermissionsAuthorizedInAggregateCommand(aggregateId, Metadata))
                .ThenExpectReply<SuccessExecutionResult>(x => x.IsSuccess);
        }

        [Fact]
        public void CommandHasPermissionsFailedAttributeTest()
        {
            var aggregateId = TestPermissionsId.New;
            FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasPermissionsAttributeFailedCommand(aggregateId, Metadata))
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }

        [Fact]
        public void CommandHasPermissionFailedAttributeTest()
        {
            var aggregateId = TestPermissionsId.New;
            FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasPermissionAttributeFailedCommand(aggregateId, Metadata))
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }

        [Fact]
        public void CommandHasPermissionsSuccessAttributeTest()
        {
            var aggregateId = TestPermissionsId.With("testpermissions-a381244e-b611-4a76-ace6-b50a4c5bb777");
            FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasPermissionsAttributeSuccessCommand(aggregateId, Metadata))
                .ThenExpectReply<SuccessExecutionResult>(x => x.IsSuccess);
        }

        [Fact]
        public void CommandHasPermissionSuccessAttributeTest()
        {
            var aggregateId = TestPermissionsId.With("testpermissions-a381244e-b611-4a76-ace6-b50a4c5bb777");
            FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasPermissionAttributeSuccessCommand(aggregateId, Metadata))
                .ThenExpectReply<SuccessExecutionResult>(x => x.IsSuccess);
        }

        [Fact]
        public void CommandHasPermissionsSuccessAttributeWithObjectTest()
        {
            var aggregateId = TestPermissionsId.With("testpermissions-a381244e-b611-4a76-ace6-b50a4c5bb777");
            FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasObjectPermissionAttributeSuccessCommand(aggregateId, Metadata))
                .ThenExpectReply<SuccessExecutionResult>(x => x.IsSuccess);
        }

        [Fact]
        public void CommandHasPermissionsFailAttributeWithObjectTest()
        {
            var aggregateId = TestPermissionsId.With("testpermissions-a381244e-b611-4a76-ace6-b50a4c5bb888");
            FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasObjectPermissionAttributeSuccessCommand(aggregateId, Metadata))
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }

        private readonly CommandMetadata Metadata = new CommandMetadata(new List<KeyValuePair<String, String>>
        {
            new KeyValuePair<String, String>(MetadataKeys.UserId,"user-a381244e-b611-4a76-ace6-b50a4c5bb0f9")
        });
    }
}