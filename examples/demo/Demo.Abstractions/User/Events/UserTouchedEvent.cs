using EventFly.Aggregates;

namespace Demo.User.Events
{
    public class UserTouchedEvent : AggregateEvent<UserId> { }
}