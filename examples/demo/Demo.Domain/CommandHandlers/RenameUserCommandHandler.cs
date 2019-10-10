using Akka.Actor;
using Akkatecture.Commands;
using Akkatecture.Commands.ExecutionResults;
using Demo.Commands;

namespace Demo.Domain.CommandHandlers
{
    public class RenameUserCommandHandler : CommandHandler<UserAggregate,UserId, RenameUserCommand>
    {
        public override IExecutionResult HandleCommand(UserAggregate aggregate, IActorContext context, RenameUserCommand command)
        {
            aggregate.Rename(command.UserName);
            return ExecutionResult.Success();
        }
    }
}
