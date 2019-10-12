// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.QueryDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using Akka.Actor;
using System;

namespace Akkatecture.Definitions
{
  public class QueryDefinition : IQueryDefinition
  {
    public string Name { get; }

    public Type Type { get; }

    public IActorRef Manager { get; }

    public QueryDefinition(Type queryType, IActorRef manager)
    {
      this.Type = queryType;
      this.Name = queryType.Name;
      this.Manager = manager;
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}
