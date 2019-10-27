using System;

namespace EventFly.Validation
{
    public class ValidatorAttribute : Attribute
    {
        public Type ValidatorType { get; }

        public ValidatorAttribute(Type validatorType)
        {
            ValidatorType = validatorType;
        }
    }
}