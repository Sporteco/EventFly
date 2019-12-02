using System;

namespace EventFly.Definitions
{
    public interface IDomainEventSubscriberDefinition
    {
        Type Type { get; }
    }
}