using Akkatecture.Aggregates;
using Akkatecture.Core.VersionedTypes;
using System;

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
