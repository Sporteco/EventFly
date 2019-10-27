using System.Linq;
using EventFly.Validation;
using EventFly.ValueObjects;
using FluentValidation;

namespace Demo.ValueObjects
{
    [Validator(typeof(UserNameValidator))]
    public class UserName : SingleValueObject<string>
    {
        public UserName(string value) : base(value){}
    }
    public class UserNameValidator : AbstractValidator<UserName>
    {
        public UserNameValidator()
        {
            RuleFor(p => p.Value).NotNull();
            RuleFor(p => p.Value).Must(s => s.All(char.IsLetter));
        }
    }
}