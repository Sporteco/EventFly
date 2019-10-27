using System;
using System.Collections.Generic;
using FluentValidation;

namespace EventFly.Validation
{
    public static class CommandValidators  {
        public static IRuleBuilderOptions<T, TElement> ApplyRegisteredValidators<T, TElement>
            (this IRuleBuilder<T, TElement> ruleBuilder, IServiceProvider serviceProvider) {
            
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            var validators = (IEnumerable<IValidator<TElement>>)serviceProvider.GetService(typeof(IEnumerable<IValidator<TElement>>));

            var result = ruleBuilder.Must(element => true);
            foreach (var validator in validators)
            {
                result = ruleBuilder.SetValidator(validator);
            }

            return result;
        }
    }
}
