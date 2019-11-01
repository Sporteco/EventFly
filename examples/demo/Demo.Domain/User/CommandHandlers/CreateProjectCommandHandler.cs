using System;
using System.Threading.Tasks;
using Demo.Domain.User.Services;
using Demo.User.Commands;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;

namespace Demo.Domain.User.CommandHandlers
{
    public sealed class CreateProjectCommandHandler : AsyncCommandHandler<UserAggregate, UserId, CreateProjectCommand>
    {
        public override async Task<IExecutionResult> Handle(UserAggregate user, CreateProjectCommand cmd)
        {
            try
            {
                await Resolve<ProjectService>().Create(user, cmd.ProjectId, cmd.Name);
            }
            catch(Exception e)
            {
                return ExecutionResult.Failed(e.Message);
            }

            return ExecutionResult.Success();
        }
    }
}