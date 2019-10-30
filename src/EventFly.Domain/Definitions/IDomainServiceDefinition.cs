using System;

namespace EventFly.Definitions
{
    public interface IDomainServiceDefinition
    {
        Type Type { get; }
        IDomainServiceManagerDefinition ManagerDefinition { get; }
    }
}