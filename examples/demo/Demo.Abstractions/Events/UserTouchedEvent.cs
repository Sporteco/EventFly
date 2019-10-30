using EventFly.Aggregates;

namespace Demo.Events
{
    public class UserTouchedEvent : AggregateEvent<UserId> { }
}