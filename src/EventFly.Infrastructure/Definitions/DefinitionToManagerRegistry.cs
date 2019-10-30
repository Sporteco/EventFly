using Akka.Actor;
using EventFly.Aggregates;
using EventFly.Extensions;
using EventFly.Jobs;
using EventFly.Queries;
using EventFly.Sagas.AggregateSaga;
using EventFly.Schedulers.Commands;
using EventFly.Schedulers.Events;
using System.Collections.Generic;
using System.Linq;
using EventFly.DomainService;

namespace EventFly.Definitions
{
    internal sealed class DefinitionToManagerRegistry : IDefinitionToManagerRegistry
    {
        public IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> DefinitionToAggregateManager { get; }
        public IReadOnlyDictionary<IQueryManagerDefinition, IActorRef> DefinitionToQueryManager { get; }
        public IReadOnlyDictionary<ISagaManagerDefinition, IActorRef> DefinitionToSagaManager { get; }
        public IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> DefinitionToReadModelManager { get; }
        public IReadOnlyDictionary<IJobManagerDefinition, IActorRef> DefinitionToJobManager { get; }
        public IReadOnlyDictionary<IDomainServiceManagerDefinition, IActorRef> DefinitionToDomainServiceManager { get; }

        public DefinitionToManagerRegistry(ActorSystem actorSystem, IApplicationDefinition applicationDefinition)
        {
            _system = actorSystem;
            DefinitionToAggregateManager = RegisterAggregateManagers(applicationDefinition.Aggregates.Select(a => a.ManagerDefinition).ToList());
            DefinitionToQueryManager = RegisterQueryManagers(applicationDefinition.Queries.Select(a => a.ManagerDefinition).ToList());
            DefinitionToReadModelManager = RegisterReadModelManagers(applicationDefinition.ReadModels.Select(a => a.ManagerDefinition).ToList());
            DefinitionToSagaManager = RegisterSagaManagers(applicationDefinition.Sagas.Select(a => a.ManagerDefinition).ToList());
            DefinitionToDomainServiceManager = RegisterDomainServiceManagers(applicationDefinition.DomainServices.Select(a => a.ManagerDefinition).ToList());
            DefinitionToJobManager = RegisterJobManagers(applicationDefinition.Jobs.Select(a => a.ManagerDefinition).ToList());

            RegisterCommandsScheduler();
            RegisterEventsScheduler();
        }

        public IReadOnlyDictionary<IJobManagerDefinition, IActorRef> RegisterJobManagers(IReadOnlyCollection<IJobManagerDefinition> definitions)
        {
            var dictionaryJob = new Dictionary<IJobManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var type = typeof(JobManager<,,,>);
                var generic = type.MakeGenericType(new[] { managerDef.JobSchedulreType, managerDef.JobRunnerType, managerDef.JobType, managerDef.IdentityType });
                var manager = _system.ActorOf(Props.Create(generic), $"job-{managerDef.IdentityType.Name}-manager");
                dictionaryJob.Add(managerDef, manager);
            }
            return dictionaryJob;
        }

        public void RegisterCommandsScheduler()
        {
            _system.ActorOf(Props.Create(typeof(JobManager<PublishCommandJobScheduler, PublishCommandJobRunner, PublishCommandJob, PublishCommandJobId>)), $"job-commands-publisher-manager");
        }

        public void RegisterEventsScheduler()
        {
            _system.ActorOf(Props.Create(typeof(JobManager<PublishEventJobScheduler, PublishEventJobRunner, PublishEventJob, PublishEventJobId>)), $"job-events-publisher-manager");
        }

        private readonly ActorSystem _system;

        private IReadOnlyDictionary<IAggregateManagerDefinition, IActorRef> RegisterAggregateManagers(IReadOnlyCollection<IAggregateManagerDefinition> definitions)
        {
            var dictionaryAggregate = new Dictionary<IAggregateManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var type = typeof(AggregateManager<,>);
                var generics = type.MakeGenericType(new[] { managerDef.AggregateType, managerDef.IdentityType });
                var manager = _system.ActorOf(Props.Create(generics), $"aggregate-{managerDef.AggregateType.GetAggregateName()}-manager");
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
                var manager = _system.ActorOf(Props.Create(generics), $"query-{managerDef.QueryType.Name}-manager");
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
                var manager = _system.ActorOf(Props.Create(generics), $"saga-{managerDef.IdentityType.Name}-manager");
                dictionarySaga.Add(managerDef, manager);
            }
            return dictionarySaga;
        }

        private IReadOnlyDictionary<IDomainServiceManagerDefinition, IActorRef> RegisterDomainServiceManagers(IReadOnlyCollection<IDomainServiceManagerDefinition> definitions)
        {
            var dictionary = new Dictionary<IDomainServiceManagerDefinition, IActorRef>();
            foreach (var definition in definitions)
            {
                var type = typeof(DomainServiceManager<>).MakeGenericType(definition.ServiceType);
                var actor = _system.ActorOf(Props.Create(type), $"service-{definition.ServiceType.Name}-manager");
                dictionary.Add(definition, actor);
            }
            return dictionary;
        }

        private IReadOnlyDictionary<IReadModelManagerDefinition, IActorRef> RegisterReadModelManagers(IReadOnlyCollection<IReadModelManagerDefinition> definitions)
        {
            var dictionaryReadModel = new Dictionary<IReadModelManagerDefinition, IActorRef>();
            foreach (var managerDef in definitions)
            {
                var manager = _system.ActorOf(Props.Create(managerDef.ReadModelManagerType), $"readmodel-{managerDef.ReadModelType}-manager");
                dictionaryReadModel.Add(managerDef, manager);
            }
            return dictionaryReadModel;
        }
    }
}