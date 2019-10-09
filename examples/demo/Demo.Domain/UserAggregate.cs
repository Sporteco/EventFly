using Akkatecture.Aggregates;
using Demo.Commands;
using Demo.Events;

namespace Demo.Domain
{
    public class UserAggregate : EventSourcedAggregateRoot<UserAggregate, UserId, UserState>,
    IExecute<CreateUserCommand>
    {
        public UserAggregate(UserId id) : base(id){}
        public bool Execute(CreateUserCommand cmd)
        {
            if (!IsNew) return false;
            
            Emit(new UserCreatedEvent(cmd.UserName, cmd.Birth));
            return true;
        }
    }
}
