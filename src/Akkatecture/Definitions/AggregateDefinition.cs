using System;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;

namespace Akkatecture.Meta
{
    public interface IAggregateDefinition
    {
        AggregateName Name { get; }
        Type Type { get; }
        Type IdentityType { get; }
        IDomainDefinition Domain { get; }
        IActorRef Manager { get; }
        
    }
    public class AggregateDefinition : IAggregateDefinition
    {
        public AggregateName Name { get; }
        public Type Type { get; }
        public Type IdentityType { get; }

        public IDomainDefinition Domain { get; private set; }
        public IActorRef Manager { get; }

        public AggregateDefinition(IDomainDefinition domain, Type aggregateType, Type identityType,
            IActorRef manager)
           
        {
            Type = aggregateType;
            IdentityType = identityType;
            Name = Type.GetAggregateName();
            Domain = domain;
            Manager = manager;
        }

        public override string ToString() => Name.Value;
    }
}