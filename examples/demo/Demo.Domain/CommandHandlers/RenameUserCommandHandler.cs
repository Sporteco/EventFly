using Demo.Commands;
using Demo.Domain.Aggregates;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;

namespace Demo.Domain.CommandHandlers
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