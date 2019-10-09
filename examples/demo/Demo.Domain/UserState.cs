using Akkatecture.Aggregates;
using Demo.Commands;
using Demo.Events;
using Demo.ValueObjects;

namespace Demo.Domain
{
    public class UserState : AggregateState<UserAggregate, UserId>,
        IApply<UserCreatedEvent>
    {
        public UserName Name { get; private set; }
        public Birth Birth { get; private set; }

        public void Apply(UserCreatedEvent e)
        {
            Name = e.Name;
            Birth = e.Birth;
        }
    }
}