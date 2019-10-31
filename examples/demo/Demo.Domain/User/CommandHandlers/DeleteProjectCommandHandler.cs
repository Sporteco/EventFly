using System;
using System.Threading.Tasks;
using Demo.User.Services;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;

namespace Demo.User.CommandHandlers
{
    public sealed class DeleteProjectCommandHandler : AsyncCommandHandler<UserAggregate, UserId, DeleteProjectCommand>
    {
        public override async Task<IExecutionResult> Handle(UserAggregate user, DeleteProjectCommand cmd)
        {
            try
            {
                await Resolve<ProjectService>().Delete(user, cmd.ProjectId);
            }
            catch (Exception e)
            {
                return ExecutionResult.Failed(e.Message);
            }

            return ExecutionResult.Success();
        }
    }
}