using System;
using System.Collections.Generic;
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
        IApply<ProjectCreatedEvent>
    {
        public LocalizedString Name { get; private set; }
        public Birth Birth { get; private set; }
        public String Notes { get; private set; } = String.Empty;

        private readonly ICollection<Entities.Project> _projects = new List<Entities.Project>();
        public IEnumerable<Entities.Project> Projects => _projects;

        public void Apply(UserCreatedEvent e) { (Name, Birth) = (e.Name, e.Birth); }
        public void Apply(UserRenamedEvent e) { Name = e.NewName; }
        public void Apply(UserNotesChangedEvent e) { Notes = e.NewValue; }
        public void Apply(UserTouchedEvent _) { }

        public void Apply(ProjectCreatedEvent e) => _projects.Add(new Entities.Project(e.ProjectId, e.Name));
    }

    public class UserAggregate : EventDrivenAggregateRoot<UserAggregate, UserId, UserState>,
        IExecute<CreateUserCommand, UserId>,
        IExecute<ChangeUserNotesCommand, UserId>,
        IExecute<TrackUserTouchingCommand, UserId>
    {
        public UserAggregate(UserId id) : base(id)
        {
            Command<RenameUserCommand>();
            CommandAsync<CreateProjectCommand>();
            CommandAsync<DeleteProjectCommand>();
        }

        internal void CreateProject(ProjectId projectId, ProjectName projectName)
        {
            Emit(new ProjectCreatedEvent(Id, projectId, projectName));
        }

        internal void DeleteProject(ProjectId projectId)
        {
            Emit(new ProjectDeletedEvent(Id, projectId));
        }

        public IExecutionResult Execute(CreateUserCommand cmd)
        {
            SecurityContext.Authorized();
            SecurityContext.HasPermissions(cmd.AggregateId, DemoContext.TestUserPermission);

            Emit(new UserCreatedEvent(cmd.UserName, cmd.Birth));
            return ExecutionResult.Success();
        }

        public IExecutionResult Execute(ChangeUserNotesCommand cmd)
        {
            Emit(new UserNotesChangedEvent(Id, State.Notes, cmd.NewValue));
            return ExecutionResult.Success();
        }

        public IExecutionResult Execute(TrackUserTouchingCommand cmd)
        {
            Emit(new UserTouchedEvent());
            return ExecutionResult.Success();
        }

        public void Rename(UserName newName)
        {
            Emit(new UserRenamedEvent(newName));
        }
    }
}