using System;

namespace EventFly.Definitions
{
    public interface IDomainServiceManagerDefinition
    {
        Type ServiceType { get; }
        Type ServiceLocatorType { get; }
        Type IdentityType { get; }
    }
}