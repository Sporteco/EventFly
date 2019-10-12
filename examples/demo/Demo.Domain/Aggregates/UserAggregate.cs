using Akkatecture.Aggregates;
using Akkatecture.Commands.ExecutionResults;
using Demo.Commands;
using Demo.Domain.CommandHandlers;
using Demo.Events;
using Demo.ValueObjects;

namespace Demo.Domain.Aggregates
{
    public class UserAggregate : EventDrivenAggregateRoot<UserAggregate, UserId, UserState>,
    IExecute<CreateUserCommand,UserId>
    {
        public UserAggregate(UserId id) : base(id)
        {
            Command<RenameUserCommand,RenameUserCommandHandler>();
        }
        public IExecutionResult Execute(CreateUserCommand cmd)
        {
            Emit(new UserCreatedEvent(cmd.UserName, cmd.Birth));
            return ExecutionResult.Success();
        }

        public void Rename(UserName newName)
        {
            Emit(new UserRenamedEvent(newName));
        }
    }
}
