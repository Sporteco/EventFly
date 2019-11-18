using EventFly.Commands;
using EventFly.Validation;
using FluentValidation;

namespace EventFly.TestHelpers.Aggregates.Commands
{
    [Validator(typeof(ValidatedCommandValidator))]
    public sealed class ValidatedCommand : Command<TestAggregateId>
    {
        public ValidatedCommand(TestAggregateId aggregateId, System.Boolean isValid, CommandId sourceId) : base(aggregateId, new CommandMetadata(sourceId))
        {
            IsValid = isValid;
        }

        public System.Boolean IsValid { get; }

    }

    public sealed class ValidatedCommandValidator : AbstractValidator<ValidatedCommand>
    {
        public ValidatedCommandValidator()
        {
            RuleFor(x => x.IsValid).Must(x => x);
        }
    }
}