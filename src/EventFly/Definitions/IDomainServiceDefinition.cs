using System;

namespace EventFly.Definitions
{
    public interface IDomainServiceDefinition 
    {
        Type Type { get; }

        Type IdentityType { get; }
        IDomainServiceManagerDefinition ManagerDefinition { get; }
    }
}