using Demo.ValueObjects;
using EventFly.Commands;

namespace Demo.Project
{
    public sealed class CreateCommand : Command<ProjectId>
    {
        public CreateCommand(ProjectId projectId, ProjectName projectName) : base(projectId)
        {
            ProjectName = projectName;
        }

        public ProjectName ProjectName { get; }
    }
}
