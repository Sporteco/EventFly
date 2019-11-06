using Demo.Domain.Project.Commands;
using Demo.Domain.Project.Events;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Commands.ExecutionResults;

namespace Demo.Domain.Project
{
    public interface IProjectState : IAggregateState<ProjectId>
    {
        bool IsDeleted { get; }
        ProjectName ProjectName { get; }

        int SaveTimings();
    }

    public sealed class ProjectAggregate : EventDrivenAggregateRoot<ProjectAggregate, ProjectId, IProjectState>,
        IExecute<CreateCommand, ProjectId>,
        IExecute<DeleteCommand, ProjectId>
    {
        public ProjectAggregate(ProjectId id) : base(id) { }

        internal void Create(ProjectName projectName)
        {
            if (State.SaveTimings() > 0)
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
