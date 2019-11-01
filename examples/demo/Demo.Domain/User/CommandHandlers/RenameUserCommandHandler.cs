using Demo.User.Commands;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;

namespace Demo.Domain.User.CommandHandlers
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