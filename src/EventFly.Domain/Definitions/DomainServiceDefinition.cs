using System;

namespace EventFly.Definitions
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