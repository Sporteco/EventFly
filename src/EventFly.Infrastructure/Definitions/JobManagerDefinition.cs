using System;
using EventFly.Definitions;

namespace EventFly.Infrastructure.Definitions
{
    internal sealed class JobManagerDefinition : IJobManagerDefinition
    {
        internal JobManagerDefinition(Type jobRunnerType, Type jobSchedulreType, Type jobType, Type identityType)
        {
            JobRunnerType = jobRunnerType;
            JobSchedulreType = jobSchedulreType;
            JobType = jobType;
            IdentityType = identityType;
        }

        public Type JobRunnerType { get; }
        public Type JobSchedulreType { get; }
        public Type JobType { get; }
        public Type IdentityType { get; }
    }
}
