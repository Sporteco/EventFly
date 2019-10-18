// Decompiled with JetBrains decompiler
// Type: EventFly.Definitions.SnapshotDefinitions
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using System;
using EventFly.Aggregates.Snapshot;
using EventFly.Core.VersionedTypes;

namespace EventFly.Definitions
{
  public class SnapshotDefinitions : VersionedTypeDefinitions<IAggregateSnapshot, SnapshotVersionAttribute, SnapshotDefinition>, ISnapshotDefinitions
  {
    protected override SnapshotDefinition CreateDefinition(
      int version,
      Type type,
      string name)
    {
      return new SnapshotDefinition(version, type, name);
    }
  }
}
