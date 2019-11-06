using Demo.Domain.User;
using Demo.User.Commands;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;

namespace Demo.Application.RenameUser
{
    public class RenameUserCommandHandler : CommandHandler<UserAggregate, UserId, RenameUserCommand>
    {
        public override IExecutionResult Handle(UserAggregate aggregate, RenameUserCommand command)
        {
            aggregate.Rename(command.UserName);
            return ExecutionResult.Success();
        }
    }
}