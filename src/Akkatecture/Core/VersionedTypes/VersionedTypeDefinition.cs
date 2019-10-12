// Decompiled with JetBrains decompiler
// Type: Akkatecture.Core.VersionedTypes.VersionedTypeDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using Akkatecture.Exceptions;
using Akkatecture.ValueObjects;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Akkatecture.Core.VersionedTypes
{
  public abstract class VersionedTypeDefinition : ValueObject
  {
    public int Version { get; }

    public Type Type { get; }

    public string Name { get; }

    protected VersionedTypeDefinition(int version, Type type, string name)
    {
      this.Version = version;
      this.Type = type;
      this.Name = name;
    }

    public override string ToString()
    {
      return string.Format("{0} v{1} ({2} - {3})", (object) this.Name, (object) this.Version, (object) this.Type.GetTypeInfo().Assembly.GetName().Name, (object) this.Type.PrettyPrint());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return (object) this.Version;
      yield return (object) this.Type;
      yield return (object) this.Name;
    }
  }
}
