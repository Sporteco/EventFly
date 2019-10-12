// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.AggregateDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;
using System;

namespace Akkatecture.Definitions
{
  public class AggregateDefinition : IAggregateDefinition
  {
    public AggregateName Name { get; }

    public Type Type { get; }

    public Type IdentityType { get; }

    public IActorRef Manager { get; }

    public AggregateDefinition(Type aggregateType, Type queryIdentity, IActorRef manager)
    {
      this.Type = aggregateType;
      this.IdentityType = queryIdentity;
      this.Name = this.Type.GetAggregateName();
      this.Manager = manager;
    }

    public override string ToString()
    {
      return this.Name.Value;
    }
  }
}
