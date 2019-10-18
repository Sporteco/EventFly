using EventFly.Aggregates;
using Demo.ValueObjects;

namespace Demo.Events
{
    public class UserCreatedEvent : AggregateEvent<UserId>
    {
        public readonly UserName Name;
        public readonly Birth Birth;

        public UserCreatedEvent(UserName name, Birth birth)
        {
            Name = name;
            Birth = birth;
        }
    }
}