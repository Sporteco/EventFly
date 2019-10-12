using System;
using Akka.Actor;
using Akkatecture.Extensions;

namespace Akkatecture.Meta
{
    public class QueryDefinition : IQueryDefinition
    {
        public string Name { get; }
        public Type Type { get; }
        public Type IdentityType { get; }
    
        public IActorRef Manager { get; }
    
        public QueryDefinition(Type aggregateType, Type identityType, IActorRef manager)
               
        {
            Type = aggregateType;
            IdentityType = identityType;
            Name = Type.GetAggregateName();
            Manager = manager;
        }
    
        public override string ToString() => Name;
    }
}