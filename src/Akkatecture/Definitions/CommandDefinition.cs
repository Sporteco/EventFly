using System;
using System.Reflection;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Meta
{
    public class CommandDefinition : VersionedTypeDefinition
    {
        public bool IsInternal { get; }
        public CommandDefinition(int version,Type commandType,string name) : base(version, commandType, name)
        {
            IsInternal = CheckIsInternal(commandType);
        }

        private static bool CheckIsInternal(Type commandType)
        {
            return commandType.GetTypeInfo().GetCustomAttribute<InternalCommandAttribute>() != null;
        }

    }
}