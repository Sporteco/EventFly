// Decompiled with JetBrains decompiler
// Type: EventFly.Definitions.AggregateDefinition
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using System;
using Akka.Actor;
using EventFly.Aggregates;
using EventFly.Extensions;

namespace EventFly.Definitions
{
    public class AggregateDefinition : IAggregateDefinition
    {
        public AggregateName Name { get; }

        public Type Type { get; }

        public Type IdentityType { get; }

        public IAggregateManagerDefinition ManagerDefinition { get; }

        public AggregateDefinition(
            Type aggregateType,
            Type queryIdentity,
            IAggregateManagerDefinition managerDefinition)
        {
            Type = aggregateType;
            IdentityType = queryIdentity;
            Name = Type.GetAggregateName();
            ManagerDefinition = managerDefinition;
        }

        public override string ToString()
        {
            return Name.Value;
        }
    }
}
