// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.EventDefinitions
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using Akkatecture.Aggregates;
using Akkatecture.Core.VersionedTypes;
using System;
using Akkatecture.Events;

namespace Akkatecture.Definitions
{
  public class EventDefinitions : VersionedTypeDefinitions<IAggregateEvent, EventVersionAttribute, EventDefinition>, IEventDefinitions, IVersionedTypeDefinitions<EventVersionAttribute, EventDefinition>
  {
    protected override EventDefinition CreateDefinition(
      int version,
      Type type,
      string name)
    {
      return new EventDefinition(version, type, name);
    }
  }
}
