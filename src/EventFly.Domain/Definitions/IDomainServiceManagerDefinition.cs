using System;

namespace EventFly.Domain
{
    public interface IDomainServiceManagerDefinition
    {
        Type ServiceType { get; }
    }
}