// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.SagaDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using System;
using Akka.Actor;

namespace Akkatecture.Definitions
{
    public class SagaDefinition : AggregateDefinition, ISagaDefinition
    {
        public SagaDefinition(Type aggregateType, Type queryIdentity, ISagaManagerDefinition managerDefinition)
          : base(aggregateType, queryIdentity, managerDefinition)
        {
        }

        ISagaManagerDefinition ISagaDefinition.ManagerDefinition => (ISagaManagerDefinition)ManagerDefinition;
    }
}
