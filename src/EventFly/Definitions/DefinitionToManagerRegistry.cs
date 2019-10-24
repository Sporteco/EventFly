using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using EventFly.Aggregates;
using EventFly.DomainService;
using EventFly.Extensions;
using EventFly.Queries;
using EventFly.Sagas.AggregateSaga;

namespace EventFly.Definitions
{
    internal sealed class DefinitionToManagerRegistry : IDefinitionToManagerRegistry
    {
        private readonly ActorSystem System;
        public DefinitionToManagerRegistry(
            ActorSystem actorSystem,
            IApplicationDefinition applicationDefinition
        )
        {
            System = actorSystem;
            DefinitionToAggregateManager = RegisterAggregateManagers(applicationDefinition.Aggregates.Select(a => a.ManagerDefinition).ToList());
            DefinitionToQueryManager = RegisterQueryManagers(applicationDefinition.Queries.Select(a => a.ManagerDefinition).ToList());
            DefinitionToReadModelManager = RegisterReadModelManagers(applicationDefinition.ReadModels.Select(a => a.ManagerDefinition).ToList());
            DefinitionToSagaManager = RegisterSagaManagers(applicationDefinition.Sagas.Select(a => a.ManagerDefinition).ToList());
            DefinitionToDomainServiceManager = RegisterDomainServiceManagers(applicationDefinition.DomainServices.Select(a => a.ManagerDefinition).ToList());
        }

        public IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> DefinitionToAggregateManager { get; }
        public IReadOnlyDictionary<IQueryManagerDefinition, IActorRef> DefinitionToQueryManager { get; }
        public IReadOnlyDictionary<ISagaManagerDefinition, IActorRef> DefinitionToSagaManager { get; }
        public IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> DefinitionToReadModelManager { get; }
        public IReadOnlyDictionary<IDomainServiceManagerDefinition, IActorRef> DefinitionToDomainServiceManager { get; }

        private IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> RegisterAggregateManagers(IReadOnlyCollection<IAggregateManagerDefinition> definitions)
        {
            var dictionaryAggregate = new Dictionary<IAggregateManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var type = typeof(AggregateManager<,>);
                var generics = type.MakeGenericType(new[] { managerDef.AggregateType, managerDef.IdentityType });

                var manager = System.ActorOf(
                    Props.Create(generics),
                    $"aggregate-{managerDef.AggregateType.GetAggregateName()}-manager"
                );
                dictionaryAggregate.Add(managerDef, manager);
            }

            return dictionaryAggregate;
        }

        private IReadOnlyDictionary<IQueryManagerDefinition, IActorRef> RegisterQueryManagers(IReadOnlyCollection<IQueryManagerDefinition> definitions)
        {
            var dictionaryQuery = new Dictionary<IQueryManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var type = typeof(QueryManager<,,>);
                var generics = type.MakeGenericType(new[] { managerDef.QueryHandlerType, managerDef.QueryType, managerDef.ResultType });

                var manager = System.ActorOf(Props.Create(generics),
                    $"query-{managerDef.QueryType.Name}-manager");

                dictionaryQuery.Add(managerDef, manager);
            }

            return dictionaryQuery;
        }

        private IReadOnlyDictionary<ISagaManagerDefinition, IActorRef> RegisterSagaManagers(IReadOnlyCollection<ISagaManagerDefinition> definitions)
        {
            var dictionarySaga = new Dictionary<ISagaManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var type = typeof(AggregateSagaManager<,,>);
                var generics = type.MakeGenericType(new[] { managerDef.AggregateType, managerDef.IdentityType, managerDef.SagaLocatorType });

                var manager = System.ActorOf(
                    Props.Create(generics),
                    $"saga-{managerDef.IdentityType.Name}-manager"
                );
                dictionarySaga.Add(managerDef, manager);
            }
            return dictionarySaga;
        }

        private IReadOnlyDictionary<IDomainServiceManagerDefinition, IActorRef> RegisterDomainServiceManagers(IReadOnlyCollection<IDomainServiceManagerDefinition> definitions)
        {
            var dictionaryDomainService = new Dictionary<IDomainServiceManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var type = typeof(DomainServiceManager<,,>);
                var generics = type.MakeGenericType(managerDef.ServiceType, managerDef.IdentityType, managerDef.ServiceLocatorType);

                var manager = System.ActorOf(
                    Props.Create(generics),
                    $"service-{managerDef.IdentityType.Name}-manager"
                );
                dictionaryDomainService.Add(managerDef, manager);
            }
            return dictionaryDomainService;
        }
        private IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> RegisterReadModelManagers(IReadOnlyCollection<IReadModelManagerDefinition> definitions)
        {
            var dictionaryReadModel = new Dictionary<IReadModelManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var manager = System.ActorOf(
                    Props.Create(managerDef.ReadModelManagerType),
                    $"readmodel-{managerDef.ReadModelType}-manager"
                );

                dictionaryReadModel.Add(managerDef, manager);
            }
            return dictionaryReadModel;
        }
    }
}
