using System;
using System.Threading.Tasks;
using Demo.Domain.User;
using Demo.Domain.User.Services;
using Demo.User.Commands;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;

namespace Demo.Application.DeleteProject
{
    public sealed class DeleteProjectCommandHandler : CommandHandler<UserAggregate, UserId, DeleteProjectCommand>
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