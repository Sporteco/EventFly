using Demo.ValueObjects;
using EventFly.Specifications;
using System.Collections.Generic;
using System.Linq;

namespace Demo.User
{
    public sealed class IsCanCreateProjectWithNameSpecification : Specification<ProjectName>
    {
        private readonly UserAggregate _userAggregate;

        public IsCanCreateProjectWithNameSpecification(UserAggregate userAggregate)
        {
            _userAggregate = userAggregate;
        }

        protected override IEnumerable<string> IsNotSatisfiedBecause(ProjectName projectName)
        {
            if (_userAggregate.State.Projects.Any(p => p.Name == projectName))
                yield return $"Project with same name '{projectName}' already exists";
        }
    }
}
