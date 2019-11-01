using System.Collections.Generic;
using System.Linq;
using EventFly.Specifications;

namespace Demo.Domain.User.Specifications
{
    public sealed class IsCanDeleteProjectWithIdSpecification : Specification<ProjectId>
    {
        private readonly UserAggregate _userAggregate;

        public IsCanDeleteProjectWithIdSpecification(UserAggregate userAggregate)
        {
            _userAggregate = userAggregate;
        }

        protected override IEnumerable<string> IsNotSatisfiedBecause(ProjectId projectId)
        {
            if (_userAggregate.State.Projects.Any(p => p.Id == projectId) == false)
                yield return $"Project with ID '{projectId}' not found or already deleted";
        }
    }
}
