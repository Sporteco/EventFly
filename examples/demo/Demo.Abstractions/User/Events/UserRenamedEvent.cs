using Demo.ValueObjects;
using EventFly.Aggregates;

namespace Demo.User.Events
{
    public class UserRenamedEvent : AggregateEvent<UserId>
    {
        public readonly UserName NewName;

        public UserRenamedEvent(UserName newName)
        {
            NewName = newName;
        }
    }
}