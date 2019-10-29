using System.ComponentModel;
using Akka.TestKit.Xunit2;
using EventFly.Commands.ExecutionResults;
using EventFly.DependencyInjection;
using EventFly.TestFixture.Extensions;
using EventFly.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Tests.UnitTests.Permissions
{
    [Collection("PermissionsTests")]
    public class PermissionsTests : TestKit
    {
        private const string Category = "Permissions";

        public PermissionsTests(ITestOutputHelper testOutputHelper)
            : base(TestHelpers.Akka.Configuration.Config, "permissions-tests", testOutputHelper)
        {
            Sys.RegisterDependencyResolver(
                new ServiceCollection()
                    .AddEventFly(Sys)
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
    }

}
