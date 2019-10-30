using EventFly.Commands;
using EventFly.Validation;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace EventFly.TestHelpers.Aggregates.Commands
{
    [Validator(typeof(ValidatedCommandValidator))]
    public sealed class ValidatedCommand : Command<TestAggregateId>
    {
        public ValidatedCommand(TestAggregateId aggregateId, bool isValid, CommandId sourceId) : base(aggregateId, new CommandMetadata(sourceId))
        {
            IsValid = isValid;
        }

        public bool IsValid { get; }

    }

    public sealed class ValidatedCommandValidator : AbstractValidator<ValidatedCommand>
    {
        public ValidatedCommandValidator()
        {
            RuleFor(x => x.IsValid).Must(x => x);
        }
    }
}