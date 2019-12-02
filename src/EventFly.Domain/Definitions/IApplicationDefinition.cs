// Decompiled with JetBrains decompiler
// Type: EventFly.Definitions.IApplicationDefinition
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using EventFly.Permissions;
using System.Collections.Generic;

namespace EventFly.Definitions
{

    public interface IApplicationDefinition
    {
        IReadOnlyCollection<IContextDefinition> Contexts { get; }

        IReadOnlyCollection<IQueryDefinition> Queries { get; }

        IReadOnlyCollection<IAggregateDefinition> Aggregates { get; }

        IReadOnlyCollection<IReadModelDefinition> ReadModels { get; }

        IReadOnlyCollection<ISagaDefinition> Sagas { get; }
        IReadOnlyCollection<IJobDefinition> Jobs { get; }
        IReadOnlyCollection<IDomainServiceDefinition> DomainServices { get; }
        IReadOnlyCollection<IPermissionDefinition> Permissions { get; }
        IReadOnlyCollection<IDomainEventSubscriberDefinition> DomainEventSubscribers { get; }

        ICommandDefinitions Commands { get; }

        IEventDefinitions Events { get; }
        IEventDefinitions PublicEvents { get; }
        IEventDefinitions PrivateEvents { get; }
        IEventDefinitions ExternalEvents { get; }

        ISnapshotDefinitions Snapshots { get; }
    }
}
