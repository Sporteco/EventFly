using Demo.Domain.Project.Commands;
using Demo.Domain.Project.Events;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Commands.ExecutionResults;
using System.Threading.Tasks;

namespace Demo.Domain.Project
{
    public interface IProjectState : IAggregateState<ProjectId>
    {
        System.Boolean IsDeleted { get; }
        ProjectName ProjectName { get; }

        System.Int32 SaveTimings();
    }

    public sealed class ProjectAggregate : EventDrivenAggregateRoot<ProjectAggregate, ProjectId, IProjectState>,
        IExecute<CreateCommand, ProjectId>,
        IExecute<DeleteCommand, ProjectId>
    {
        public ProjectAggregate(ProjectId id) : base(id) { }

        internal async Task Create(ProjectName projectName)
        {
            if (State.SaveTimings() > 0)
                await Emit(new CreatedEvent(Id, projectName));
        }

        internal async Task Delete()
        {
            await Emit(new DeletedEvent(Id));
        }

        #region Command handlers

        public async Task<IExecutionResult> Execute(CreateCommand cmd)
        {
            await Create(cmd.ProjectName);
            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> Execute(DeleteCommand command)
        {
            await Delete();
            return ExecutionResult.Success();
        }

        #endregion
    }
}
