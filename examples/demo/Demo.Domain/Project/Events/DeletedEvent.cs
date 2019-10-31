using EventFly.Aggregates;

namespace Demo.Project
{
    public sealed class DeletedEvent : AggregateEvent<ProjectId>
    {
        public DeletedEvent(ProjectId projectId)
        {
            ProjectId = projectId;
        }

        public ProjectId ProjectId { get; }
    }
}
