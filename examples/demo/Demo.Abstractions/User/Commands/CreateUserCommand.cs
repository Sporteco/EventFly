using System;
using System.ComponentModel;
using Demo.ValueObjects;
using EventFly.Commands;
using EventFly.Localization;
using EventFly.Security;
using EventFly.Validation;
using FluentValidation;

namespace Demo.User.Commands
{
    [Description("Создание нового пользователя")]
    [HasPermissions(DemoContext.CreateUser,DemoContext.ChangeUser)]
    [Validator(typeof(CreateUserCommandValidator))]
    public class CreateUserCommand : Command<UserId>
    {
        public LocalizedString UserName { get; }
        public Birth Birth { get; }

        public CreateUserCommand(UserId aggregateId, LocalizedString userName, Birth birth) : base(aggregateId)
        {
            UserName = userName;
            Birth = birth;
        }
    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator(IServiceProvider sp)
        {
            RuleFor(p => p.AggregateId).NotNull();
            //RuleFor(p => p.UserName).SetValidator(new UserNameValidator());
            RuleFor(p => p.Birth).ApplyRegisteredValidators(sp);
          
        }
    }
}