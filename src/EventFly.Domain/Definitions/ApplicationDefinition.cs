using EventFly.Permissions;
using EventFly.Subscribers;
using System.Collections.Generic;
using System.Linq;

namespace EventFly.Definitions
{
    internal sealed class ApplicationDefinition : IApplicationDefinition
    {
        public IReadOnlyCollection<IContextDefinition> Contexts => _contexts;
        public IReadOnlyCollection<IQueryDefinition> Queries => Contexts.SelectMany(i => i.Queries.Select(q => q)).ToList();
        public IReadOnlyCollection<IAggregateDefinition> Aggregates => Contexts.SelectMany(i => i.Aggregates.Select(q => q)).ToList();
        public IReadOnlyCollection<IReadModelDefinition> ReadModels => Contexts.SelectMany(i => i.ReadModels.Select(q => q)).ToList();
        public IReadOnlyCollection<ISagaDefinition> Sagas => Contexts.SelectMany(i => i.Sagas.Select(q => q)).ToList();
        public IReadOnlyCollection<IDomainServiceDefinition> DomainServices => Contexts.SelectMany(i => i.DomainServices.Select(q => q)).ToList();
        public IReadOnlyCollection<IPermissionDefinition> Permissions => Contexts.SelectMany(i => i.Permissions.Select(q => q)).ToList();
        public IReadOnlyCollection<IJobDefinition> Jobs => Contexts.SelectMany(i => i.Jobs.Select(q => q)).ToList();
        public IReadOnlyCollection<IDomainEventSubscriberDefinition> DomainEventSubscribers
            => Contexts.SelectMany(i => i.DomainEventSubscribers).Union(_globalSubscribers).ToList();
        public IEventDefinitions Events => _events;
        public IEventDefinitions PublicEvents => _publicEvents;
        public IEventDefinitions PrivateEvents => _privateEvents;
        public IEventDefinitions ExternalEvents => _externalEvents;
        public ISnapshotDefinitions Snapshots => _snapshots;
        public ICommandDefinitions Commands => _commands;

        public void RegisterContext(IContextDefinition context)
        {
            _contexts.Add(context);
            _events.AddDefinitions(context.Events);
            _publicEvents.AddDefinitions(context.PublicEvents);
            _privateEvents.AddDefinitions(context.PrivateEvents);
            _externalEvents.AddDefinitions(context.ExternalEvents);
            _snapshots.AddDefinitions(context.Snapshots);
            _commands.AddDefinitions(context.Commands);
        }

        public void RegisterDomainEventSubscriber<TDomainEventSubscriber>()
            where TDomainEventSubscriber : DomainEventSubscriber
        {
            _globalSubscribers.Add(new DomainEventSubscriberDefinition(typeof(TDomainEventSubscriber)));
        }

        private readonly EventAggregatedDefinitions _events = new EventAggregatedDefinitions();
        private readonly EventAggregatedDefinitions _publicEvents = new EventAggregatedDefinitions();
        private readonly EventAggregatedDefinitions _privateEvents = new EventAggregatedDefinitions();
        private readonly EventAggregatedDefinitions _externalEvents = new EventAggregatedDefinitions();
        private readonly SnapshotAggregatedDefinitions _snapshots = new SnapshotAggregatedDefinitions();
        private readonly CommandAggregatedDefinitions _commands = new CommandAggregatedDefinitions();
        private readonly List<IContextDefinition> _contexts = new List<IContextDefinition>();
        private readonly List<IDomainEventSubscriberDefinition> _globalSubscribers = new List<IDomainEventSubscriberDefinition>();
    }
}