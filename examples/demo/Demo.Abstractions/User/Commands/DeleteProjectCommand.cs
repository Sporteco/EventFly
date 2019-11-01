using System.ComponentModel;
using EventFly.Commands;

namespace Demo.User.Commands
{
    [Description("Удалить проект")]
    public sealed class DeleteProjectCommand : Command<UserId>
    {
        public DeleteProjectCommand(UserId userId, ProjectId projectId) : base(userId)
        {
            ProjectId = projectId;
        }

        public ProjectId ProjectId { get; }
    }
}
