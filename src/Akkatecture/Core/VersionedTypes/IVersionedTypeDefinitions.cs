// Decompiled with JetBrains decompiler
// Type: Akkatecture.Core.VersionedTypes.IVersionedTypeDefinitions`2
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using System;
using System.Collections.Generic;

namespace Akkatecture.Core.VersionedTypes
{
  public interface IVersionedTypeDefinitions<TAttribute, TDefinition>
    where TAttribute : VersionedTypeAttribute
    where TDefinition : VersionedTypeDefinition
  {
    void Load(IReadOnlyCollection<Type> types);

    IEnumerable<TDefinition> GetDefinitions(string name);

    bool TryGetDefinition(string name, int version, out TDefinition definition);

    IEnumerable<TDefinition> GetAllDefinitions();

    TDefinition GetDefinition(string name, int version);

    TDefinition GetDefinition(Type type);

    IReadOnlyCollection<TDefinition> GetDefinitions(Type type);

    bool TryGetDefinition(Type type, out TDefinition definition);

    bool TryGetDefinitions(Type type, out IReadOnlyCollection<TDefinition> definitions);

    void Load(params Type[] types);
  }
}
