using Akka.TestKit.Xunit2;
using Demo.Infrastructure;
using Demo.ValueObjects;
using EventFly.Commands.ExecutionResults;
using EventFly.DependencyInjection;
using EventFly.TestFixture.Aggregates;
using EventFly.TestFixture.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using Demo.Domain.User;
using Demo.User.Commands;
using Demo.User.Events;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Tests
{
    public class DemoAggregateTests : TestKit
    {
        public DemoAggregateTests(ITestOutputHelper testOutputHelper) : base(Configuration.Config, "dem-aggregate-tests", testOutputHelper)
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
                .GivenNothing()
                .When(cmd)
                .ThenExpectReply<UnauthorizedAccessResult>(x => !x.IsSuccess);
        }

        [Fact]
        public void CreateProjectTest()
        {
            var aggregateId = UserId.New;
            var projectId = ProjectId.New;
            var projectName = new ProjectName("TestProject");

            //var cmd = new CreateProjectCommand(aggregateId, projectId, projectName);

            this.FixtureFor<UserAggregate, UserId>(aggregateId)
                .GivenNothing()
                .When(new CreateProjectCommand(aggregateId, projectId, projectName))
                .ThenExpectDomainEvent<ProjectCreatedEvent>(x =>
                    x.AggregateEvent.ProjectId == projectId && x.AggregateEvent.Name == projectName
                );
        }
    }
}