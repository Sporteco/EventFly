// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.ApplicationDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akkatecture.Jobs;

namespace Akkatecture.Definitions
{
    public class ApplicationDefinition : IApplicationDefinition
    {
        private readonly List<IDomainDefinition> _domains = new List<IDomainDefinition>();
        private readonly EventAggregatedDefinitions _events = new EventAggregatedDefinitions();
        private readonly JobAggregatedDefinitions _jobs = new JobAggregatedDefinitions();
        private readonly SnapshotAggregatedDefinitions _snapshots = new SnapshotAggregatedDefinitions();
        private readonly CommandAggregatedDefinitions _commands = new CommandAggregatedDefinitions();
        private readonly ActorSystem _system;

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

        public TDomainDefinition RegisterDomainDefenitions<TDomainDefinition>() where TDomainDefinition : IDomainDefinition
        {
            var instance = (IDomainDefinition)Activator.CreateInstance(typeof(TDomainDefinition), (object)_system);
            _domains.Add(instance);
            _events.AddDefinitions(instance.Events);
            _jobs.AddDefinitions(instance.Jobs);
            _snapshots.AddDefinitions(instance.Snapshots);
            _commands.AddDefinitions(instance.Commands);
            return (TDomainDefinition)instance;
        }

        public ApplicationDefinition(ActorSystem system)
        {
            _system = system;
        }
    }
}
