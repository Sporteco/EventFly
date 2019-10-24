using System.Collections.Generic;
using System.Linq;

namespace EventFly.Definitions
{

    internal sealed class ApplicationDefinition : IApplicationDefinition
    {
        private readonly EventAggregatedDefinitions _events = new EventAggregatedDefinitions();
        private readonly SnapshotAggregatedDefinitions _snapshots = new SnapshotAggregatedDefinitions();
        private readonly CommandAggregatedDefinitions _commands = new CommandAggregatedDefinitions();
        private readonly List<IContextDefinition> _contexts = new List<IContextDefinition>();

        public void RegisterContext(IContextDefinition context)
        {
            _contexts.Add(context);

            _events.AddDefinitions(context.Events);
            _snapshots.AddDefinitions(context.Snapshots);
            _commands.AddDefinitions(context.Commands);
        }

        public IReadOnlyCollection<IContextDefinition> Contexts => _contexts;

        public IReadOnlyCollection<IQueryDefinition> Queries => Contexts.SelectMany(i => i.Queries.Select(q => q)).ToList();

        public IReadOnlyCollection<IAggregateDefinition> Aggregates => Contexts.SelectMany(i => i.Aggregates.Select(q => q)).ToList();

        public IReadOnlyCollection<IReadModelDefinition> ReadModels => Contexts.SelectMany(i => i.ReadModels.Select(q => q)).ToList();

        public IReadOnlyCollection<ISagaDefinition> Sagas => Contexts.SelectMany(i => i.Sagas.Select(q => q)).ToList();

        public IReadOnlyCollection<IJobDefinition> Jobs => Contexts.SelectMany(i => i.Jobs.Select(q => q)).ToList();

        public IEventDefinitions Events => _events;

        public ISnapshotDefinitions Snapshots => _snapshots;

        public ICommandDefinitions Commands => _commands;
    }
}
