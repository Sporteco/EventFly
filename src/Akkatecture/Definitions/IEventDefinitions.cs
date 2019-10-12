using Akkatecture.Aggregates;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Definitions
{
  public interface IEventDefinitions : IVersionedTypeDefinitions<EventVersionAttribute, EventDefinition>
  {
  }
}
