using System;

namespace EventFly.Domain
{
    public interface IDomainServiceDefinition
    {
        Type Type { get; }
        IDomainServiceManagerDefinition ManagerDefinition { get; }
    }
}