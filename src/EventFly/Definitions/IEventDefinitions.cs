using EventFly.Aggregates;
using EventFly.Core.VersionedTypes;

namespace EventFly.Definitions
{
  public interface IEventDefinitions : IVersionedTypeDefinitions<EventVersionAttribute, EventDefinition>
  {
  }
}
