using System.Collections.Generic;
using System.Linq;
using EventFly.DependencyInjection;
using EventFly.Jobs;

namespace EventFly.Definitions
{
    public interface IInfrastructureDefinitions
    {
        void Describe(InfrastructureBuilder infrastructureBuilder);
    }

    public sealed class ApplicationDefinition : IApplicationDefinition
    {
        private readonly EventAggregatedDefinitions _events = new EventAggregatedDefinitions();
        private readonly JobAggregatedDefinitions _jobs = new JobAggregatedDefinitions();
        private readonly SnapshotAggregatedDefinitions _snapshots = new SnapshotAggregatedDefinitions();
        private readonly CommandAggregatedDefinitions _commands = new CommandAggregatedDefinitions();

        public ApplicationDefinition(
            IReadOnlyCollection<IDomainDefinition> domains,
            IReadOnlyCollection<IReadModelDefinition> readModelDefinitions,
            IReadOnlyCollection<ISagaDefinition> sagaDefinitions
        )
        {
            ReadModels = readModelDefinitions;
            Sagas = sagaDefinitions;
            foreach (var instance in domains)
            {
                _events.AddDefinitions(instance.Events);
                _jobs.AddDefinitions(instance.Jobs); // TODO: ?? to infrastcture layer
                _snapshots.AddDefinitions(instance.Snapshots);
                _commands.AddDefinitions(instance.Commands);
            }

            Domains = domains;
        }

        public IReadOnlyCollection<IDomainDefinition> Domains { get; }

        public IReadOnlyCollection<IQueryDefinition> Queries
        {
            get
            {
                return Domains.SelectMany(i => i.Queries.Select(q => q)).ToList();
            }
        }

        public IReadOnlyCollection<IAggregateDefinition> Aggregates
        {
            get
            {
                return Domains.SelectMany(i => i.Aggregates.Select(q => q)).ToList();
            }
        }

        public IReadOnlyCollection<IReadModelDefinition> ReadModels { get; }

        public IReadOnlyCollection<ISagaDefinition> Sagas { get; }

        public IEventDefinitions Events => _events;

        public IJobDefinitions Jobs => _jobs;

        public ISnapshotDefinitions Snapshots => _snapshots;

        public ICommandDefinitions Commands => _commands;
    }
}
