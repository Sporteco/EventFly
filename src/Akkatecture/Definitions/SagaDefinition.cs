using System;
using Akka.Actor;

namespace Akkatecture.Definitions
{
    public class SagaDefinition : AggregateDefinition, ISagaDefinition
    {
        public SagaDefinition(Type aggregateType, Type queryIdentity, IActorRef manager) : base(aggregateType, queryIdentity, manager)
        {
        }
    }
}