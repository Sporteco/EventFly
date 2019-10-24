using System;

namespace EventFly.Definitions
{
    public class DomainServiceDefinition : IDomainServiceDefinition
    {

        public Type Type { get; }

        public Type IdentityType { get; }

        public IDomainServiceManagerDefinition ManagerDefinition { get; }

        public DomainServiceDefinition(
            Type aggregateType,
            Type queryIdentity,
            IDomainServiceManagerDefinition managerDefinition)
        {
            Type = aggregateType;
            IdentityType = queryIdentity;
            ManagerDefinition = managerDefinition;
        }
    }
}