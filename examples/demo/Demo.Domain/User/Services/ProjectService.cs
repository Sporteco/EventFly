using Demo.ValueObjects;
using EventFly.DomainService;
using System;
using System.Threading.Tasks;

namespace Demo.User.Services
{
    public sealed class ProjectService : BaseDomainService<ProjectService>
    {
        public async Task Create(UserAggregate user, ProjectId projectId, ProjectName projectName)
        {
            var spec = new IsCanCreateProjectWithNameSpecification(user);
            if (spec.IsSatisfiedBy(projectName) == false)
                throw new InvalidOperationException(string.Join("\n", spec.WhyIsNotSatisfiedBy(projectName)));

            var result = await CommandBus.Publish(new Project.CreateCommand(projectId, projectName));
            if (result.IsSuccess == false)
                throw new InvalidOperationException(result.ToString());

            user.CreateProject(projectId, projectName);
        }

        public async Task Delete(UserAggregate user, ProjectId projectId)
        {
            var spec = new IsCanDeleteProjectWithIdSpecification(user);
            if (spec.IsSatisfiedBy(projectId) == false)
                throw new InvalidOperationException(string.Join("\n", spec.WhyIsNotSatisfiedBy(projectId)));

            var result = await CommandBus.Publish(new Project.DeleteCommand(projectId));
            if (result.IsSuccess == false)
                throw new InvalidOperationException(result.ToString());

            user.DeleteProject(projectId);
        }
    }
}
