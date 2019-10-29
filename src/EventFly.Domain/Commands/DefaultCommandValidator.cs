using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.Commands
{
    public class DefaultCommandValidator : ICommandValidator
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultCommandValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public ValidationResult Validate(ICommand command)
        {
            //TODO: Reflection & cache
            var validatorType = typeof(IValidator<>).MakeGenericType(command.GetType());
            var validators = _serviceProvider.GetServices(validatorType).Cast<IValidator>().ToList();

            if (validators.Any())
            {
                //TODO: Parallel
                foreach (var validator in validators)
                {
                    var result = validator.Validate(command);
                    if (!result.IsValid)
                        return result;
                }
            }
            return new ValidationResult();
        }

        public int Priority => 100;
    }
}