using Demo.Domain.User;
using Demo.Infrastructure;
using Demo.User.Commands;
using Demo.User.Events;
using Demo.ValueObjects;
using EventFly.Commands.ExecutionResults;
using EventFly.TestFixture;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Tests
{
    public class DemoAggregateTests : AggregateTestKit<UserContext>
    {
        public DemoAggregateTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void CreateEventCommandTest()
        {
            var aggregateId = UserId.New;
            var cmd = new CreateUserCommand(aggregateId, new UserName(("Test", "ru")), new Birth(DateTime.Today));

            FixtureFor<UserAggregate, UserId>(aggregateId)
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

            FixtureFor<UserAggregate, UserId>(aggregateId)
                .GivenNothing()
                .When(new CreateProjectCommand(aggregateId, projectId, projectName))
                .ThenExpectDomainEvent<ProjectCreatedEvent>(x =>
                    x.AggregateEvent.ProjectId == projectId && x.AggregateEvent.Name == projectName
                );
        }
    }
}