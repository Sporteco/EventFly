using System.Collections.Generic;
using System.Linq;
using EventFly.Jobs;

namespace EventFly.Definitions
{
    public sealed class ApplicationDefinition : IApplicationDefinition
    {
        private readonly IReadOnlyList<IDomainDefinition> _domains = new List<IDomainDefinition>();
        private readonly EventAggregatedDefinitions _events = new EventAggregatedDefinitions();
        private readonly JobAggregatedDefinitions _jobs = new JobAggregatedDefinitions();
        private readonly SnapshotAggregatedDefinitions _snapshots = new SnapshotAggregatedDefinitions();
        private readonly CommandAggregatedDefinitions _commands = new CommandAggregatedDefinitions();

        public ApplicationDefinition(IReadOnlyList<IDomainDefinition> domains)
        {
            foreach(var instance in domains)
            {
                _events.AddDefinitions(instance.Events);
                _jobs.AddDefinitions(instance.Jobs);
                _snapshots.AddDefinitions(instance.Snapshots);
                _commands.AddDefinitions(instance.Commands);
            }

            _domains = domains;
        }

        public IReadOnlyCollection<IDomainDefinition> Domains
        {
            get
            {
                return _domains;
            }
        }

        public IReadOnlyCollection<IQueryDefinition> Queries
        {
            get
            {
                return _domains.SelectMany(i => i.Queries.Select(q => q)).ToList();
            }
        }

        public IReadOnlyCollection<IAggregateDefinition> Aggregates
        {
            get
            {
                return _domains.SelectMany(i => i.Aggregates.Select(q => q)).ToList();
            }
        }

        public IReadOnlyCollection<IReadModelDefinition> ReadModels
        {
            get
            {
                return _domains.SelectMany(i => i.ReadModels.Select(q => q)).ToList();
            }
        }

        public IReadOnlyCollection<ISagaDefinition> Sagas
        {
            get
            {
                return _domains.SelectMany(i => i.Sagas.Select(q => q)).ToList();
            }
        }

        public IEventDefinitions Events
        {
            get
            {
                return _events;
            }
        }

        public IJobDefinitions Jobs
        {
            get
            {
                return _jobs;
            }
        }

        public ISnapshotDefinitions Snapshots
        {
            get
            {
                return _snapshots;
            }
        }

        public ICommandDefinitions Commands
        {
            get
            {
                return _commands;
            }
        }
    }
}
