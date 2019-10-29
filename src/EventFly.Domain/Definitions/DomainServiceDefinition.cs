using System;

namespace EventFly.Domain
{
    public class DomainServiceDefinition : IDomainServiceDefinition
    {
        public Type Type { get; }
        public Type IdentityType { get; }
        public IDomainServiceManagerDefinition ManagerDefinition { get; }

        public DomainServiceDefinition(Type type, Type identityType, IDomainServiceManagerDefinition managerDefinition)
        {
            Type = type;
            IdentityType = identityType;
            ManagerDefinition = managerDefinition;
        }
    }
}