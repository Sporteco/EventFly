// Decompiled with JetBrains decompiler
// Type: EventFly.Core.VersionedTypes.IVersionedTypeDefinitions`2
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using System;
using System.Collections.Generic;

namespace EventFly.Core.VersionedTypes
{
    public interface IVersionedTypeDefinitions<TAttribute, TDefinition>
    where TAttribute : VersionedTypeAttribute
    where TDefinition : VersionedTypeDefinition
    {
        void Load(IReadOnlyCollection<Type> types);

        IEnumerable<TDefinition> GetDefinitions(String name);

        Boolean TryGetDefinition(String name, Int32 version, out TDefinition definition);

        IEnumerable<TDefinition> GetAllDefinitions();

        TDefinition GetDefinition(String name, Int32 version);

        TDefinition GetDefinition(Type type);

        IReadOnlyCollection<TDefinition> GetDefinitions(Type type);

        Boolean TryGetDefinition(Type type, out TDefinition definition);

        Boolean TryGetDefinitions(Type type, out IReadOnlyCollection<TDefinition> definitions);

        void Load(params Type[] types);
    }
}
