// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.IAggregateDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using Akka.Actor;
using Akkatecture.Aggregates;
using System;

namespace Akkatecture.Definitions
{
  public interface IAggregateDefinition
  {
    AggregateName Name { get; }

    Type Type { get; }

    Type IdentityType { get; }

    IActorRef Manager { get; }
  }
}
