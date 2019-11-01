using EventFly.Commands;

namespace Demo.Domain.Project.Commands
{
    public sealed class DeleteCommand : Command<ProjectId>
    {
        public DeleteCommand(ProjectId projectId) : base(projectId) { }
    }
}
