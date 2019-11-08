using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.User.Commands;
using Demo.User.Events;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Commands.ExecutionResults;
using EventFly.Localization;

namespace Demo.Domain.User
{
    public class UserState : AggregateState<UserAggregate, UserId>,
        IApply<UserCreatedEvent>,
        IApply<UserRenamedEvent>,
        IApply<UserNotesChangedEvent>,
        IApply<UserTouchedEvent>,
        IApply<ProjectCreatedEvent>,
        IApply<ProjectDeletedEvent>
    {
        public LocalizedString Name { get; private set; }
        public Birth Birth { get; private set; }
        public String Notes { get; private set; } = String.Empty;

        private readonly ICollection<Entities.Project> _projects = new List<Entities.Project>();
        public IEnumerable<Entities.Project> Projects => _projects;

        public async Task Apply(UserCreatedEvent e) { (Name, Birth) = (e.Name, e.Birth); }
        public async Task Apply(UserRenamedEvent e) { Name = e.NewName; }
        public async Task Apply(UserNotesChangedEvent e) { Notes = e.NewValue; }
        public async Task Apply(UserTouchedEvent _) { }

        public async Task Apply(ProjectCreatedEvent e) => _projects.Add(new Entities.Project(e.ProjectId, e.Name));
        public async Task Apply(ProjectDeletedEvent aggregateEvent)
        {
            var projectToRemove = _projects.FirstOrDefault(i => i.Id == aggregateEvent.ProjectId);
            if (projectToRemove != null)
                _projects.Remove(projectToRemove);
        }
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

        internal void CreateProject(ProjectId projectId, ProjectName projectName)
        {
            Emit(new ProjectCreatedEvent(Id, projectId, projectName)).GetAwaiter().GetResult();
        }

        internal void DeleteProject(ProjectId projectId)
        {
            Emit(new ProjectDeletedEvent(Id, projectId)).GetAwaiter().GetResult();
        }

        public async Task<IExecutionResult> Execute(CreateUserCommand cmd)
        {
            //SecurityContext.Authorized();
            //SecurityContext.HasPermissions(cmd.AggregateId, DemoContext.TestUserPermission);

            Emit(new UserCreatedEvent(cmd.UserName, cmd.Birth)).GetAwaiter().GetResult();
            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> Execute(ChangeUserNotesCommand cmd)
        {
            Emit(new UserNotesChangedEvent(Id, State.Notes, cmd.NewValue)).GetAwaiter().GetResult();
            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> Execute(TrackUserTouchingCommand cmd)
        {
            Emit(new UserTouchedEvent()).GetAwaiter().GetResult();
            return ExecutionResult.Success();
        }

        public void Rename(UserName newName)
        {
            Emit(new UserRenamedEvent(newName)).GetAwaiter().GetResult();
        }
    }
}