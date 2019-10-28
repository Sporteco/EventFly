using System.Linq;
using EventFly.Localization;
using EventFly.Validation;
using FluentValidation;

namespace Demo.ValueObjects
{
    [Validator(typeof(UserNameValidator))]
    public class UserName : LocalizedString
    {
        public UserName(params StringLocalization[] locs) : base(locs){}

        public static implicit operator UserName((string value, LanguageCode lang) str)
        {
            return new UserName(str);
        }
    }
    public class UserNameValidator : AbstractValidator<UserName>
    {
        public UserNameValidator()
        {
            RuleFor(p => p.Locs).Must(p => p.Any());
        }
    }
}