using System.Collections.Generic;
using Akka.Actor;

namespace EventFly.Definitions
{
    public sealed class DefinitionToManagerRegistry : IDefinitionToManagerRegistry
    {
        public DefinitionToManagerRegistry(
            IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> definitionToAggregateManager,
            IReadOnlyDictionary<IQueryManagerDefinition, IActorRef> definitionToQueryManager,
            IReadOnlyDictionary<ISagaManagerDefinition, IActorRef> definitionToSagaManager,
            IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> definitionToReadModelManager,
            IReadOnlyDictionary<IJobManagerDefinition, IActorRef> definitionToJobManager
        )
        {
            DefinitionToAggregateManager = definitionToAggregateManager;
            DefinitionToQueryManager = definitionToQueryManager;
            DefinitionToSagaManager = definitionToSagaManager;
            DefinitionToReadModelManager = definitionToReadModelManager;
            DefinitionToJobManager = definitionToJobManager;
        }

        public IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> DefinitionToAggregateManager { get; }
        public IReadOnlyDictionary<IQueryManagerDefinition, IActorRef> DefinitionToQueryManager { get; }
        public IReadOnlyDictionary<ISagaManagerDefinition, IActorRef> DefinitionToSagaManager { get; }
        public IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> DefinitionToReadModelManager { get; }
        public IReadOnlyDictionary<IJobManagerDefinition, IActorRef> DefinitionToJobManager { get; }
    }
}
