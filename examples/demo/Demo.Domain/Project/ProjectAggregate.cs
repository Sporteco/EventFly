using Demo.Domain.Project.Commands;
using Demo.Domain.Project.Events;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Commands.ExecutionResults;

namespace Demo.Domain.Project
{
    public sealed class ProjectAggregateState : AggregateState<ProjectAggregate, ProjectId>,
        IApply<CreatedEvent>,
        IApply<DeletedEvent>
    {
        public ProjectName ProjectName { get; private set; }
        public bool IsDeleted { get; private set; }

        public void Apply(CreatedEvent e)
        {
            ProjectName = e.Name;
        }

        public void Apply(DeletedEvent _)
        {
            IsDeleted = true;
        }
    }

    public sealed class ProjectAggregate : EventDrivenAggregateRoot<ProjectAggregate, ProjectId, ProjectAggregateState>,
        IExecute<CreateCommand, ProjectId>,
        IExecute<DeleteCommand, ProjectId>
    {
        public ProjectAggregate(ProjectId id) : base(id) { }

        internal void Create(ProjectName projectName)
        {
            Emit(new CreatedEvent(Id, projectName));
        }

        internal void Delete()
        {
            Emit(new DeletedEvent(Id));
        }

        #region Command handlers

        public IExecutionResult Execute(CreateCommand cmd)
        {
            Create(cmd.ProjectName);

            return ExecutionResult.Success();
        }

        public IExecutionResult Execute(DeleteCommand command)
        {
            Delete();

            return ExecutionResult.Success();
        }

        #endregion
    }
}
