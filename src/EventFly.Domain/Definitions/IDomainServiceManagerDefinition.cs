using System;

namespace EventFly.Domain
{
    public interface IDomainServiceManagerDefinition
    {
        Type ServiceType { get; }
        Type ServiceLocatorType { get; }
        Type IdentityType { get; }
    }
}