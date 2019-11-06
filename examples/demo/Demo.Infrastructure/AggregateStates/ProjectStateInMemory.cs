using System.Threading.Tasks;
using Demo.Domain.Project;
using Demo.Domain.Project.Events;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Storages.EntityFramework;

namespace Demo.Infrastructure.AggregateStates
{
    public sealed class ProjectStateInMemory : AggregateState<ProjectAggregate, ProjectId>, IProjectState,
        IApply<CreatedEvent>,
        IApply<DeletedEvent>
    {
        public ProjectName ProjectName { get; private set; }
        public int SaveTimings()
        {
            throw new System.NotImplementedException();
        }

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

    public sealed class ProjectState : EntityFrameworkAggregateState<ProjectAggregate, ProjectId, DemoDbContext>, IProjectState,
        IApply<CreatedEvent>,
        IApply<DeletedEvent>
    {
        private ProjectModel _model;

        public bool IsDeleted => _model.IsDeleted;
        public ProjectName ProjectName => _model.ProjectName;
        public int SaveTimings()
        {
            return 0;
        }

        public override async Task LoadState(ProjectId id)
        {
            _model = await DbContext.Projects.FindAsync(id);
            if (_model == null)
            {
                _model = new ProjectModel();
                DbContext.Add(_model);
            }
        }

        public void Apply(CreatedEvent aggregateEvent)
        {
            _model.Id = aggregateEvent.ProjectId;
            _model.ProjectName = aggregateEvent.Name;

            DbContext.SaveChanges();
        }

        public void Apply(DeletedEvent aggregateEvent)
        {
            _model.IsDeleted = true;
            DbContext.SaveChanges();
        }

    }
}
