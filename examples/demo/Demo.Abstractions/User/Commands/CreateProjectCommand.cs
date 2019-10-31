using Demo.ValueObjects;
using EventFly.Commands;
using System.ComponentModel;

namespace Demo.User
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
