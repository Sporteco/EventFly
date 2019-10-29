using System;
using EventFly.Extensions;
using EventFly.Jobs;

namespace EventFly.Definitions
{
    internal class JobDefinition : IJobDefinition
    {
        internal JobDefinition(Type type, Type identityType, IJobManagerDefinition managerDefinition)
        {
            Type = type;
            Name = type.GetJobName();
            IdentityType = identityType;
            ManagerDefinition = managerDefinition;
        }

        public JobName Name { get; }

        public Type Type { get; }

        public Type IdentityType { get; }

        public IJobManagerDefinition ManagerDefinition { get; }
    }
}