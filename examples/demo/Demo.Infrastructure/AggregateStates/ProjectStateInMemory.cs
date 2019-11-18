using Demo.Domain.Project;
using Demo.Domain.Project.Events;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Storages.EntityFramework;
using System.Threading.Tasks;

namespace Demo.Infrastructure.AggregateStates
{
    public sealed class ProjectStateInMemory : AggregateState<ProjectStateInMemory, ProjectId>, IProjectState,
        IApply<CreatedEvent>,
        IApply<DeletedEvent>
    {
        public ProjectName ProjectName { get; private set; }
        public System.Boolean IsDeleted { get; private set; }

        public System.Int32 SaveTimings() => 0;

        public void Apply(CreatedEvent e)
        {
            ProjectName = e.Name;
        }

        public void Apply(DeletedEvent _)
        {
            IsDeleted = true;
        }
    }

    public sealed class ProjectState : EntityFrameworkAggregateState<ProjectState, ProjectId, DemoDbContext>, IProjectState,
        IApply<CreatedEvent>,
        IApply<DeletedEvent>
    {
        public ProjectState(DemoDbContext dbContext) : base(dbContext) { }

        public System.Boolean IsDeleted => _model.IsDeleted;
        public ProjectName ProjectName => _model.ProjectName;
        public System.Int32 SaveTimings() => 0;

        public override async Task LoadState(ProjectId id)
        {
            if (_model == null)
            {
                _model = new ProjectModel();
                await DbContext.AddAsync(_model);
            }
        }

        public void Apply(CreatedEvent aggregateEvent)
        {
            _model.Id = aggregateEvent.ProjectId;
            _model.ProjectName = aggregateEvent.Name;
        }

        public void Apply(DeletedEvent aggregateEvent)
        {
            _model.IsDeleted = true;
        }

        private ProjectModel _model;
    }
}