using System;

namespace EventFly.Domain
{
    public class DomainServiceDefinition : IDomainServiceDefinition
    {
        public Type Type { get; }
        public IDomainServiceManagerDefinition ManagerDefinition { get; }

        public DomainServiceDefinition(Type type, IDomainServiceManagerDefinition managerDefinition)
        {
            Type = type;
            ManagerDefinition = managerDefinition;
        }
    }
}