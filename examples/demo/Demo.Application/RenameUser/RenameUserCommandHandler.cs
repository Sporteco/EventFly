using Demo.Domain.User;
using Demo.User.Commands;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using System.Threading.Tasks;

namespace Demo.Application.RenameUser
{
    public class RenameUserCommandHandler : CommandHandler<UserAggregate, UserId, RenameUserCommand>
    {
        public override async Task<IExecutionResult> Handle(UserAggregate aggregate, RenameUserCommand command)
        {
            aggregate.Rename(command.UserName);
            return ExecutionResult.Success();
        }
    }
}