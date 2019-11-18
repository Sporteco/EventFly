// Decompiled with JetBrains decompiler
// Type: EventFly.Core.VersionedTypes.VersionedTypeDefinition
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using EventFly.Exceptions;
using EventFly.ValueObjects;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EventFly.Core.VersionedTypes
{
    public abstract class VersionedTypeDefinition : ValueObject
    {
        public Int32 Version { get; }

        public Type Type { get; }

        public String Name { get; }

        protected VersionedTypeDefinition(Int32 version, Type type, String name)
        {
            Version = version;
            Type = type;
            Name = GetName(name);
        }
        private String GetName(String name)
            => name.EndsWith("command", StringComparison.InvariantCultureIgnoreCase) ? name.Substring(0, name.Length - "command".Length) : name;


        public override String ToString()
        {
            return $"{ Name} v{(Object)Version} ({(Object)Type.GetTypeInfo().Assembly.GetName().Name} - {(Object)Type.PrettyPrint()})";
        }

        protected override IEnumerable<Object> GetEqualityComponents()
        {
            yield return Version;
            yield return Type;
            yield return Name;
        }
    }
}
