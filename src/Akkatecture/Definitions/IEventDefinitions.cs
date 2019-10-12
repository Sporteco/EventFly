using Akkatecture.Core.VersionedTypes;
using Akkatecture.Events;

namespace Akkatecture.Definitions
{
  public interface IEventDefinitions : IVersionedTypeDefinitions<EventVersionAttribute, EventDefinition>
  {
  }
}
