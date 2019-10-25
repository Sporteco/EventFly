using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using Demo.Commands;
using Demo.Domain.Aggregates;

namespace Demo.Domain.CommandHandlers
{
    public class RenameUserCommandHandler : CommandHandler<UserAggregate,UserId, RenameUserCommand>
    {
        public override IExecutionResult Handle(UserAggregate aggregate, RenameUserCommand command)
        {
            aggregate.Rename(command.UserName);
            return ExecutionResult.Success();
        }
    }
}
