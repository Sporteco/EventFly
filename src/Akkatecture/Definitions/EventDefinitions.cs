using System;
using Akkatecture.Aggregates;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Definitions
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
