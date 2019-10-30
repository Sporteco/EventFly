using Akka.TestKit.Xunit2;
using Demo.Commands;
using Demo.Domain.Aggregates;
using Demo.Infrastructure;
using Demo.ValueObjects;
using EventFly.Commands.ExecutionResults;
using EventFly.DependencyInjection;
using EventFly.TestFixture.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Tests
{
    public class DemoAggregateTests : TestKit
    {
        public DemoAggregateTests(ITestOutputHelper testOutputHelper) : base(Configuration.Config, "dem-aggregate-tests", testOutputHelper)
        {
            new ServiceCollection()
                .AddEventFly(Sys)
                .WithContext<UserContext>()
                .Services
                .BuildServiceProvider()
                .UseEventFly();
        }

        [Fact]
        public void CreateEventCommandTest()
        {
            var aggregateId = UserId.New;
            var cmd = new CreateUserCommand(aggregateId, new UserName(("Test", "ru")), new Birth(DateTime.Today));

            this.FixtureFor<UserAggregate, UserId>(aggregateId)
                .GivenNothing()
                .When(cmd)
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }
    }
}