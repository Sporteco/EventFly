using EventFly.Domain;
using System;

namespace EventFly.Definitions
{
    internal sealed class DomainServiceManagerDefinition : IDomainServiceManagerDefinition
    {
        public Type ServiceType { get; }
        public Type IdentityType { get; }
        public Type ServiceLocatorType { get; }

        public DomainServiceManagerDefinition(Type serviceType, Type identityType, Type serviceLocatorType)
        {
            ServiceType = serviceType;
            IdentityType = identityType;
            ServiceLocatorType = serviceLocatorType;
        }
    }
}