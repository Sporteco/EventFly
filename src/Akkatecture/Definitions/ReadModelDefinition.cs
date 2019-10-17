// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.ReadModelDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using System;
using Akka.Actor;

namespace Akkatecture.Definitions
{
    public class ReadModelDefinition : IReadModelDefinition
    {
        public string Name { get; }

        public Type Type { get; }

        public IReadModelManagerDefinition ManagerDefinition { get; }

        public ReadModelDefinition(Type readModelType, IReadModelManagerDefinition managerDefinition)
        {
            Type = readModelType;
            Name = readModelType.Name;
            ManagerDefinition = managerDefinition;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
