using Demo.ValueObjects;
using EventFly.Entities;

namespace Demo.User.Entities
{
    public sealed class Project : Entity<ProjectId>
    {
        public Project(ProjectId id, ProjectName name) : base(id)
        {
            Name = name;
        }

        public ProjectName Name { get; } 
    }
}
