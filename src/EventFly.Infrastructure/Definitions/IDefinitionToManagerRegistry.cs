using System.Collections.Generic;
using Akka.Actor;
using EventFly.Definitions;

namespace EventFly.Infrastructure.Definitions
{
    public interface IDefinitionToManagerRegistry
    {
        IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> DefinitionToAggregateManager { get; }
        IReadOnlyDictionary<IQueryManagerDefinition, IActorRef> DefinitionToQueryManager { get; }
        IReadOnlyDictionary<ISagaManagerDefinition, IActorRef> DefinitionToSagaManager { get; }
        IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> DefinitionToReadModelManager { get; }
        IReadOnlyDictionary<IJobManagerDefinition, IActorRef> DefinitionToJobManager { get; }
        IReadOnlyDictionary<IDomainServiceManagerDefinition, IActorRef> DefinitionToDomainServiceManager { get; }
    }
}
