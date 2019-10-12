// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.AggregateDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using System;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;

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
      Type = aggregateType;
      IdentityType = queryIdentity;
      Name = Type.GetAggregateName();
      Manager = manager;
    }

    public override string ToString()
    {
      return Name.Value;
    }
  }
}
