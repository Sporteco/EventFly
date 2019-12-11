using System;
using EventFly.Definitions;

namespace EventFly.Infrastructure.Definitions
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