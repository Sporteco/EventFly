using System;

namespace EventFly.Definitions
{
    internal sealed class DomainServiceManagerDefinition : IDomainServiceManagerDefinition
    {
        public DomainServiceManagerDefinition(Type aggregateType, Type identityType, Type serviceLocatorType)
        {
            ServiceType = aggregateType;
            IdentityType = identityType;
            ServiceLocatorType = serviceLocatorType;
        }

        public Type ServiceType { get; }

        public Type IdentityType { get; }
        public Type ServiceLocatorType { get; }
    }
}