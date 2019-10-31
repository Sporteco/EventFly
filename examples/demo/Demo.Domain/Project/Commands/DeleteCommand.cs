using EventFly.Commands;

namespace Demo.Project
{
    public sealed class DeleteCommand : Command<ProjectId>
    {
        public DeleteCommand(ProjectId projectId) : base(projectId) { }
    }
}
