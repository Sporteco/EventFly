// Decompiled with JetBrains decompiler
// Type: EventFly.Definitions.IQueryDefinition
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using System;
using Akka.Actor;

namespace EventFly.Definitions
{
    public interface IQueryDefinition
    {
        string Name { get; }

        Type Type { get; }

        Type QueryResultType { get; }
        IQueryManagerDefinition ManagerDefinition { get; }
    }
}
