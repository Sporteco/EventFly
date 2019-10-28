using System;
using Akka.TestKit.Xunit2;
using Demo.Commands;
using Demo.Domain.Aggregates;
using Demo.Events;
using Demo.Infrastructure;
using Demo.ValueObjects;
using EventFly.Commands.ExecutionResults;
using EventFly.DependencyInjection;
using EventFly.TestFixture.Aggregates;
using EventFly.TestFixture.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Tests
{
    [Collection("DemoAggregateTests")]
    public class DemoAggregateTests : TestKit
    {
        public DemoAggregateTests(ITestOutputHelper testOutputHelper)
            : base(Configuration.Config, "dem-aggregate-tests", testOutputHelper)
        {
            new ServiceCollection()
                .AddTestEventFly(Sys)
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
                .Given(cmd)
                .When(cmd)
                .ThenExpect<UserCreatedEvent>(e => e.Name != null && e.Name == cmd.UserName)
                .ThenExpectReply<ExecutionResult>(x => x.IsSuccess);

        }
    }
}
