using EventFly.Aggregates;

namespace Demo.User.Events
{
    public class UserNotesChangedEvent : AggregateEvent<UserId>
    {
        public UserId UserId { get; }
        public System.String OldValue { get; }
        public System.String NewValue { get; }

        public UserNotesChangedEvent(UserId userId, System.String oldValue, System.String newValue)
        {
            UserId = userId;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}