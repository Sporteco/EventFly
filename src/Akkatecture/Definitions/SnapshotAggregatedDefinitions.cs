// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.SnapshotAggregatedDefinitions
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using Akkatecture.Aggregates.Snapshot;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Definitions
{
  public class SnapshotAggregatedDefinitions : AggregatedDefinitions<SnapshotDefinitions, SnapshotVersionAttribute, SnapshotDefinition>, ISnapshotDefinitions, IVersionedTypeDefinitions<SnapshotVersionAttribute, SnapshotDefinition>
  {
  }
}
