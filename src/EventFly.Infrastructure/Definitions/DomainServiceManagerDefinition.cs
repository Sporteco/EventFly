using System;

namespace EventFly.Definitions
{
    internal sealed class DomainServiceManagerDefinition : IDomainServiceManagerDefinition
    {
        public Type ServiceType { get; }

        public DomainServiceManagerDefinition(Type serviceType)
        {
            ServiceType = serviceType;
        }
    }
}