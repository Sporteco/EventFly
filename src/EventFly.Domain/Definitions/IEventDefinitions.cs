using EventFly.Aggregates;
using EventFly.Core.VersionedTypes;
using EventFly.Definitions;
using EventFly.Domain.VersionedTypes;

namespace EventFly.Domain.Definitions
{
    public interface IEventDefinitions : IVersionedTypeDefinitions<EventVersionAttribute, EventDefinition>
    {
    }
}
