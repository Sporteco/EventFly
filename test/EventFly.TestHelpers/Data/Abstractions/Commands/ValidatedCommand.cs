using EventFly.Commands;
using EventFly.Validation;
using FluentValidation;
using System;

namespace EventFly.Tests.Abstractions
{
    [Validator(typeof(ValidatedCommandValidator))]
    public sealed class ValidatedCommand : Command<TestAggregateId>
    {
        public ValidatedCommand(TestAggregateId aggregateId, Boolean isValid, CommandId sourceId) : base(aggregateId, new CommandMetadata(sourceId))
        {
            IsValid = isValid;
        }

        public Boolean IsValid { get; }
    }

    public sealed class ValidatedCommandValidator : AbstractValidator<ValidatedCommand>
    {
        public ValidatedCommandValidator()
        {
            RuleFor(x => x.IsValid).Must(x => x);
        }
    }
}