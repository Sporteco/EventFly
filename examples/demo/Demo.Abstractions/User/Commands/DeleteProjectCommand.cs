using EventFly.Commands;
using System.ComponentModel;

namespace Demo.User
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
