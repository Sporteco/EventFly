using Akka.TestKit.Xunit2;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.DependencyInjection;
using EventFly.Metadata;
using EventFly.TestFixture.Aggregates;
using EventFly.TestFixture.Extensions;
using EventFly.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Tests.UnitTests.Permissions
{
    [Collection("PermissionsTests")]
    public class PermissionsTests : TestKit
    {
        private const System.String Category = "Permissions";
        private readonly CommandMetadata Metadata = new CommandMetadata(new List<KeyValuePair<System.String, System.String>>
        {
            new KeyValuePair<System.String, System.String>(MetadataKeys.UserId,"user-a381244e-b611-4a76-ace6-b50a4c5bb0f9")
        });

        public PermissionsTests(ITestOutputHelper testOutputHelper)
            : base(TestHelpers.Akka.Configuration.Config, "permissions-tests", testOutputHelper)
        {
            Sys.RegisterDependencyResolver(
                new ServiceCollection()
                    .AddTestEventFly(Sys)
                    .WithContext<TestPermissionsContext>()
                    .Services
                    .BuildServiceProvider()
                    .UseEventFly()
            );
        }

        [Fact]
        [Category(Category)]
        public void CommandAuthorizedAttributeTest()
        {
            var aggregateId = TestPermissionsId.New;

            this.FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestPermissionsAuthorizedAttributeCommand(aggregateId))
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }
        [Fact]
        [Category(Category)]
        public void CommandAuthorizedAttributeWithUserTest()
        {
            var aggregateId = TestPermissionsId.New;

            this.FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestPermissionsAuthorizedAttributeCommand(aggregateId, Metadata))
                .ThenExpectReply<SuccessExecutionResult>(x => x.IsSuccess);
        }
        [Fact]
        [Category(Category)]
        public void CommandAuthorizedInAggregateTest()
        {
            var aggregateId = TestPermissionsId.New;

            this.FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestPermissionsAuthorizedInAggregateCommand(aggregateId))
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }
        [Fact]
        [Category(Category)]
        public void CommandAuthorizedInAggregateWithUserTest()
        {
            var aggregateId = TestPermissionsId.New;

            this.FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestPermissionsAuthorizedInAggregateCommand(aggregateId, Metadata))
                .ThenExpectReply<SuccessExecutionResult>(x => x.IsSuccess);
        }

        [Fact]
        [Category(Category)]
        public void CommandHasPermissionsFailedAttributeTest()
        {
            var aggregateId = TestPermissionsId.New;

            this.FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasPermissionsAttributeFailedCommand(aggregateId, Metadata))
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }
        [Fact]
        [Category(Category)]
        public void CommandHasPermissionFailedAttributeTest()
        {
            var aggregateId = TestPermissionsId.New;

            this.FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasPermissionAttributeFailedCommand(aggregateId, Metadata))
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }
        [Fact]
        [Category(Category)]
        public void CommandHasPermissionsSuccessAttributeTest()
        {
            var aggregateId = TestPermissionsId.With("testpermissions-a381244e-b611-4a76-ace6-b50a4c5bb777");

            this.FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasPermissionsAttributeSuccessCommand(aggregateId, Metadata))
                .ThenExpectReply<SuccessExecutionResult>(x => x.IsSuccess);
        }
        [Fact]
        [Category(Category)]
        public void CommandHasPermissionSuccessAttributeTest()
        {
            var aggregateId = TestPermissionsId.With("testpermissions-a381244e-b611-4a76-ace6-b50a4c5bb777");

            this.FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasPermissionAttributeSuccessCommand(aggregateId, Metadata))
                .ThenExpectReply<SuccessExecutionResult>(x => x.IsSuccess);
        }

        [Fact]
        [Category(Category)]
        public void CommandHasPermissionsSuccessAttributeWithObjectTest()
        {
            var aggregateId = TestPermissionsId.With("testpermissions-a381244e-b611-4a76-ace6-b50a4c5bb777");

            this.FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasObjectPermissionAttributeSuccessCommand(aggregateId, Metadata))
                .ThenExpectReply<SuccessExecutionResult>(x => x.IsSuccess);
        }
        [Fact]
        [Category(Category)]
        public void CommandHasPermissionsFailAttributeWithObjectTest()
        {
            var aggregateId = TestPermissionsId.With("testpermissions-a381244e-b611-4a76-ace6-b50a4c5bb888");

            this.FixtureFor<TestPermissionsAggregate, TestPermissionsId>(aggregateId)
                .GivenNothing()
                .When(new TestHasObjectPermissionAttributeSuccessCommand(aggregateId, Metadata))
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }
    }

}
