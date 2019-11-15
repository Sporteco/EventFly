using Demo.User.Commands;
using Demo.User.Events;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Commands.ExecutionResults;
using EventFly.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Domain.User
{
    public class UserState : AggregateState<UserState, UserId>,
        IApply<UserCreatedEvent>,
        IApply<UserRenamedEvent>,
        IApply<UserNotesChangedEvent>,
        IApply<UserTouchedEvent>,
        IApply<ProjectCreatedEvent>,
        IApply<ProjectDeletedEvent>
    {
        public LocalizedString Name { get; private set; }
        public Birth Birth { get; private set; }
        public string Notes { get; private set; } = string.Empty;
        public IEnumerable<Entities.Project> Projects => _projects;

        public void Apply(UserCreatedEvent e) { (Name, Birth) = (e.Name, e.Birth); }
        public void Apply(UserRenamedEvent e) { Name = e.NewName; }
        public void Apply(UserNotesChangedEvent e) { Notes = e.NewValue; }
        public void Apply(UserTouchedEvent _) { }

        public void Apply(ProjectCreatedEvent e)
        {
            _projects.Add(new Entities.Project(e.ProjectId, e.Name));
        }

        public void Apply(ProjectDeletedEvent aggregateEvent)
        {
            var projectToRemove = _projects.FirstOrDefault(i => i.Id == aggregateEvent.ProjectId);
            if (projectToRemove != null) _projects.Remove(projectToRemove);
        }

        private readonly ICollection<Entities.Project> _projects = new List<Entities.Project>();
    }

    public class UserAggregate : EventDrivenAggregateRoot<UserAggregate, UserId, UserState>,
        IExecute<CreateUserCommand, UserId>,
        IExecute<ChangeUserNotesCommand, UserId>,
        IExecute<TrackUserTouchingCommand, UserId>
    {
        public UserAggregate(UserId id) : base(id)
        {
            Command<RenameUserCommand>();
            Command<CreateProjectCommand>();
            Command<DeleteProjectCommand>();
        }

        internal async Task CreateProject(ProjectId projectId, ProjectName projectName)
        {
            await Emit(new ProjectCreatedEvent(Id, projectId, projectName));
        }

        internal async Task DeleteProject(ProjectId projectId)
        {
            await Emit(new ProjectDeletedEvent(Id, projectId));
        }

        public async Task<IExecutionResult> Execute(CreateUserCommand cmd)
        {
            //SecurityContext.Authorized();
            //SecurityContext.HasPermissions(cmd.AggregateId, DemoContext.TestUserPermission);

            await Emit(new UserCreatedEvent(cmd.UserName, cmd.Birth));
            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> Execute(ChangeUserNotesCommand cmd)
        {
            await Emit(new UserNotesChangedEvent(Id, State.Notes, cmd.NewValue));
            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> Execute(TrackUserTouchingCommand cmd)
        {
            await Emit(new UserTouchedEvent());
            return ExecutionResult.Success();
        }

        public async Task Rename(UserName newName)
        {
            await Emit(new UserRenamedEvent(newName));
        }
    }
}