using EventFly.Aggregates;
using EventFly.Core.VersionedTypes;
using System;

namespace EventFly.Definitions
{
    public class EventDefinitions : VersionedTypeDefinitions<IAggregateEvent, EventVersionAttribute, EventDefinition>, IEventDefinitions
    {
        protected override EventDefinition CreateDefinition(
          Int32 version,
          Type type,
          String name)
        {
            return new EventDefinition(version, type, name);
        }
    }
}
