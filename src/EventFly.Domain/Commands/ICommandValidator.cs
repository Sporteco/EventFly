using FluentValidation.Results;

namespace EventFly.Commands
{
    public interface ICommandValidator
    {
        ValidationResult Validate(ICommand command);
        System.Int32 Priority { get; }
    }
}