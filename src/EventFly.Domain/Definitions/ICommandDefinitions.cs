using EventFly.Commands;
using EventFly.Core.VersionedTypes;
using EventFly.Definitions;
using EventFly.Domain.VersionedTypes;

namespace EventFly.Domain.Definitions
{
    public interface ICommandDefinitions : IVersionedTypeDefinitions<CommandVersionAttribute, CommandDefinition>
    {
    }
}
