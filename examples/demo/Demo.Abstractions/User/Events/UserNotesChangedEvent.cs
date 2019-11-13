using EventFly.Aggregates;

namespace Demo.User.Events
{
    public class UserNotesChangedEvent : AggregateEvent<UserId>
    {
        public UserId UserId { get; }
        public string OldValue { get; }
        public string NewValue { get; }

        public UserNotesChangedEvent(UserId userId, string oldValue, string newValue)
        {
            UserId = userId;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}