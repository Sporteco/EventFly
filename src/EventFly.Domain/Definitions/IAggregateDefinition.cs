using System;
using EventFly.Domain.Aggregates;

namespace EventFly.Domain.Definitions
{
    public interface IAggregateDefinition
    {
        AggregateName Name { get; }

        Type Type { get; }

        Type IdentityType { get; }
        IAggregateManagerDefinition ManagerDefinition { get; }
    }
}
