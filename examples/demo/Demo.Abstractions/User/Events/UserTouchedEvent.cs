using EventFly.Aggregates;

namespace Demo.User
{
    public class UserTouchedEvent : AggregateEvent<UserId> { }
}