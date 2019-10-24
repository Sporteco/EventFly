using System.Collections.Generic;
using Akka.Actor;

namespace EventFly.Definitions
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
