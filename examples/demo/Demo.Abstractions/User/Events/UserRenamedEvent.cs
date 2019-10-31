using EventFly.Aggregates;
using Demo.ValueObjects;

namespace Demo.User
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