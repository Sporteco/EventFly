using Demo.Domain.User;
using Demo.Domain.User.Services;
using Demo.User.Commands;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using System;
using System.Threading.Tasks;

namespace Demo.Application.CreateProject
{
    public sealed class CreateProjectCommandHandler : AsyncCommandHandler<UserAggregate, UserId, CreateProjectCommand>
    {
        private readonly IExternalService _externalService;

        public CreateProjectCommandHandler(IExternalService externalService)
        {
            _externalService = externalService;
        }

        public override async Task<IExecutionResult> Handle(UserAggregate user, CreateProjectCommand cmd)
        {
            await _externalService.DoAnything();

            try
            {
                await Resolve<ProjectService>().Create(user, cmd.ProjectId, cmd.Name);
            }
            catch (Exception e)
            {
                return ExecutionResult.Failed(e.Message);
            }

            return ExecutionResult.Success();
        }
    }
}
