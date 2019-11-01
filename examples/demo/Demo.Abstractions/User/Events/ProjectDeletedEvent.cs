using EventFly.Aggregates;

namespace Demo.User.Events
{
    public sealed class ProjectDeletedEvent : AggregateEvent<UserId>
    {
        public ProjectDeletedEvent(UserId userId, ProjectId projectId)
        {
            UserId = userId;
            ProjectId = projectId;
        }

        public UserId UserId { get; }
        public ProjectId ProjectId { get; }
    }
}
