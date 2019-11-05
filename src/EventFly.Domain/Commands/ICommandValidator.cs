using FluentValidation.Results;

namespace EventFly.Commands
{
    public interface ICommandValidator
    {
        ValidationResult Validate(ICommand command);
        int Priority { get; }
    }
}