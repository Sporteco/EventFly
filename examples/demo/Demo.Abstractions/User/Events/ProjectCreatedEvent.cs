using Demo.ValueObjects;
using EventFly.Aggregates;

namespace Demo.User
{
    public sealed class ProjectCreatedEvent : AggregateEvent<UserId>
    {
        public ProjectCreatedEvent(UserId userId, ProjectId projectId, ProjectName name)
        {
            UserId = userId;
            ProjectId = projectId;
            Name = name;
        }

        public UserId UserId { get; }
        public ProjectId ProjectId { get; }
        public ProjectName Name { get; }
    }
}
