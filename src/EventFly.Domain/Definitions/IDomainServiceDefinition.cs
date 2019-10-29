using System;

namespace EventFly.Domain
{
    public interface IDomainServiceDefinition
    {
        Type Type { get; }
        Type IdentityType { get; }
        IDomainServiceManagerDefinition ManagerDefinition { get; }
    }
}