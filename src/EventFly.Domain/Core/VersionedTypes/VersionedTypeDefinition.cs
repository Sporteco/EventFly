// Decompiled with JetBrains decompiler
// Type: EventFly.Core.VersionedTypes.VersionedTypeDefinition
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using EventFly.Exceptions;
using EventFly.ValueObjects;

namespace EventFly.Core.VersionedTypes
{
  public abstract class VersionedTypeDefinition : ValueObject
  {
    public int Version { get; }

    public Type Type { get; }

    public string Name { get; }

    protected VersionedTypeDefinition(int version, Type type, string name)
    {
      Version = version;
      Type = type;
      Name = name;
    }

    public override string ToString()
    {
      return $"{ Name} v{(object) Version} ({(object) Type.GetTypeInfo().Assembly.GetName().Name} - {(object) Type.PrettyPrint()})";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Version;
      yield return Type;
      yield return Name;
    }
  }
}
