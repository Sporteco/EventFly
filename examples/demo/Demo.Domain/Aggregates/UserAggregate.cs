using Demo.Commands;
using Demo.Domain.CommandHandlers;
using Demo.Events;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Commands.ExecutionResults;
using EventFly.Localization;
using System;

namespace Demo.Domain.Aggregates
{
    public class UserState : AggregateState<UserAggregate, UserId>,
        IApply<UserCreatedEvent>,
        IApply<UserRenamedEvent>,
        IApply<UserNotesChangedEvent>,
        IApply<UserTouchedEvent>
    {
        public LocalizedString Name { get; private set; }
        public Birth Birth { get; private set; }
        public String Notes { get; private set; } = String.Empty;

        public void Apply(UserCreatedEvent e) { (Name, Birth) = (e.Name, e.Birth); }
        public void Apply(UserRenamedEvent e) { Name = e.NewName; }
        public void Apply(UserNotesChangedEvent e) { Notes = e.NewValue; }
        public void Apply(UserTouchedEvent _) { }
    }

    public class UserAggregate : EventDrivenAggregateRoot<UserAggregate, UserId, UserState>,
        IExecute<CreateUserCommand, UserId>,
        IExecute<ChangeUserNotesCommand, UserId>,
        IExecute<TrackUserTouchingCommand, UserId>
    {
        public UserAggregate(UserId id) : base(id)
        {
            Command<RenameUserCommand, RenameUserCommandHandler>();
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