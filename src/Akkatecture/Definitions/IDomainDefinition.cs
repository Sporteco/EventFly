// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.IDomainDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Akkatecture.Definitions
{
    public interface IDomainDependencies<out TDomain>
        where TDomain : IDomainDefinition
    {
        IServiceCollection Dependencies { get; }
    }

    public interface IDomainDefinition
    {
        IDomainDependencies<IDomainDefinition> DomainDependencies { get; }

        string Name { get; }

        IReadOnlyCollection<IAggregateDefinition> Aggregates { get; }

        IReadOnlyCollection<ISagaDefinition> Sagas { get; }

        IReadOnlyCollection<IQueryDefinition> Queries { get; }

        IReadOnlyCollection<IReadModelDefinition> ReadModels { get; }

        EventDefinitions Events { get; }

        CommandDefinitions Commands { get; }

        SnapshotDefinitions Snapshots { get; }

        JobDefinitions Jobs { get; }
    }
}
