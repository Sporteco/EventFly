using System.ComponentModel;
using Demo.ValueObjects;
using EventFly.Commands;

namespace Demo.User.Commands
{
    [Description("Создать проект")]
    public sealed class CreateProjectCommand : Command<UserId>
    {
        public CreateProjectCommand(UserId userId, ProjectId projectId, ProjectName name) : base(userId)
        {
            ProjectId = projectId;
            Name = name;
        }

        public ProjectId  ProjectId { get; }
        public ProjectName Name { get; }
    }
}
