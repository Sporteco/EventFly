using System.ComponentModel;
using EventFly.Commands;
using Demo.ValueObjects;
using FluentValidation;

namespace Demo.Commands
{
    [Description("Создание нового пользователя")]
    public class CreateUserCommand : Command<UserId>
    {
        public UserName UserName { get; }
        public Birth Birth { get; }

        public CreateUserCommand(UserId aggregateId, UserName userName, Birth birth) : base(aggregateId)
        {
            UserName = userName;
            Birth = birth;
        }
    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(p => p.AggregateId).NotNull();
            RuleFor(p => p.UserName).SetValidator(new UserNameValidator());
            RuleFor(p => p.Birth).SetValidator(new BirthValidator());
          
        }
    }
}