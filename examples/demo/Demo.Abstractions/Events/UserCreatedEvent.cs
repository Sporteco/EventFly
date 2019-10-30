using EventFly.Aggregates;
using Demo.ValueObjects;
using EventFly.Localization;

namespace Demo.Events
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