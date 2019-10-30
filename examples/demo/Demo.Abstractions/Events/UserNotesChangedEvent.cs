using EventFly.Aggregates;
using System;

namespace Demo.Events
{
    public class UserNotesChangedEvent : AggregateEvent<UserId>
    {
        public UserId UserId { get; }
        public String OldValue { get; }
        public String NewValue { get; }

        public UserNotesChangedEvent(UserId userId, String oldValue, String newValue)
        {
            UserId = userId;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}