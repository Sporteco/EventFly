// Decompiled with JetBrains decompiler
// Type: EventFly.Definitions.IDomainDefinition
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using EventFly.Domain;
using System.Collections.Generic;

namespace EventFly.Definitions
{
    public interface IContextDefinition
    {
        string Name { get; }

        IReadOnlyCollection<IAggregateDefinition> Aggregates { get; }

        IReadOnlyCollection<IQueryDefinition> Queries { get; }
        IReadOnlyCollection<IReadModelDefinition> ReadModels { get; }
        IReadOnlyCollection<ISagaDefinition> Sagas { get; }
        IReadOnlyCollection<IJobDefinition> Jobs { get; }
        IReadOnlyCollection<IDomainServiceDefinition> DomainServices { get; }

        EventDefinitions Events { get; }

        CommandDefinitions Commands { get; }

        SnapshotDefinitions Snapshots { get; }
    }
}
