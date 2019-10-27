using System;
using EventFly.Validation;
using EventFly.ValueObjects;
using FluentValidation;

namespace Demo.ValueObjects
{
    [Validator(typeof(BirthValidator))]
    public class Birth : SingleValueObject<DateTime>
    {
        public Birth(DateTime value) : base(value){}
    }
    public class BirthValidator : AbstractValidator<Birth>
    {
        public BirthValidator()
        {
            RuleFor(p => p.Value).NotNull();
            RuleFor(p => p.Value).LessThan(DateTime.Now);
        }
    }
}