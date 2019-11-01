using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Localization;

namespace Demo.User.Events
{
    public class UserCreatedEvent : AggregateEvent<UserId>
    {
        public readonly LocalizedString Name;
        public readonly Birth Birth;

        public UserCreatedEvent(LocalizedString name, Birth birth)
        {
            Name = name;
            Birth = birth;
        }
    }
}