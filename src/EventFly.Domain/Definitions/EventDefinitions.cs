using System;
using EventFly.Aggregates;
using EventFly.Core.VersionedTypes;

namespace EventFly.Definitions
{
  public class EventDefinitions : VersionedTypeDefinitions<IAggregateEvent, EventVersionAttribute, EventDefinition>, IEventDefinitions
  {
    protected override EventDefinition CreateDefinition(
      int version,
      Type type,
      string name)
    {
      return new EventDefinition(version, type, name);
    }
  }
}
