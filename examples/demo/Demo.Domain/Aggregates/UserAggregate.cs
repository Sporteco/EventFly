using EventFly.Aggregates;
using EventFly.Commands.ExecutionResults;
using Demo.Commands;
using Demo.Events;
using Demo.ValueObjects;
using Demo.Domain.CommandHandlers;
using EventFly.Localization;

namespace Demo.Domain.Aggregates
{
    public class UserState : AggregateState<UserAggregate, UserId>,
        IApply<UserCreatedEvent>,
        IApply<UserRenamedEvent>
    {
        public LocalizedString Name { get; private set; }
        public Birth Birth { get; private set; }

        public void Apply(UserCreatedEvent e) { (Name, Birth) = (e.Name, e.Birth); }
        public void Apply(UserRenamedEvent e) { Name = e.NewName; }
    }

    public class UserAggregate : EventDrivenAggregateRoot<UserAggregate, UserId, UserState>,
        IExecute<CreateUserCommand, UserId>
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

        public void Rename(UserName newName)
        {
            Emit(new UserRenamedEvent(newName));
        }
    }
}
