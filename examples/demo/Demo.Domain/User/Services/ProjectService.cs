using Demo.Domain.Project.Commands;
using Demo.Domain.User.Specifications;
using Demo.ValueObjects;
using EventFly.DomainService;
using System;
using System.Threading.Tasks;

namespace Demo.Domain.User.Services
{
    public sealed class ProjectService : SynchronizedDomainService<ProjectService>
    {
        public async Task Create(UserAggregate user, ProjectId projectId, ProjectName projectName)
        {
            var spec = new IsCanCreateProjectWithNameSpecification(user);
            if (spec.IsSatisfiedBy(projectName) == false)
                throw new InvalidOperationException(String.Join("\n", spec.WhyIsNotSatisfiedBy(projectName)));

            var result = await CommandBus.Publish(new CreateCommand(projectId, projectName));
            if (result.IsSuccess == false)
                throw new InvalidOperationException(result.ToString());

            await user.CreateProject(projectId, projectName);
        }

        public async Task Delete(UserAggregate user, ProjectId projectId)
        {
            var spec = new IsCanDeleteProjectWithIdSpecification(user);
            if (spec.IsSatisfiedBy(projectId) == false)
                throw new InvalidOperationException(String.Join("\n", spec.WhyIsNotSatisfiedBy(projectId)));

            var result = await CommandBus.Publish(new DeleteCommand(projectId));
            if (result.IsSuccess == false)
                throw new InvalidOperationException(result.ToString());

            await user.DeleteProject(projectId);
        }
    }
}
