using Demo.Infrastructure;
using Demo.User.Commands;
using Demo.User.Events;
using EventFly.TestFixture;
using EventFly.TestFixture.Internals;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Tests
{
    public class UserTouchTrackingServiceTests : ContextTestKit<UserContext>
    {
        public UserTouchTrackingServiceTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void RegisteredDomainServiceWorking()
        {
            "When I send command to aggregate"
                .Do(new ChangeUserNotesCommand(UserId.New, "Such a nice user..."));

            "And there is a service reacting on event from this command"
                .EmptyStep();

            "Then I see event produced by command from service"
                .Expect<UserTouchedEvent>();
        }
    }
}