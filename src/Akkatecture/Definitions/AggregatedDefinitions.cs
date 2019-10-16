// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.AggregatedDefinitions`3
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Definitions
{
    public class AggregatedDefinitions<TDefinitions, TAttribute, TDefinition> : IVersionedTypeDefinitions<TAttribute, TDefinition>
      where TDefinitions : IVersionedTypeDefinitions<TAttribute, TDefinition>
      where TAttribute : VersionedTypeAttribute
      where TDefinition : VersionedTypeDefinition
    {
        private readonly List<TDefinitions> _definitions = new List<TDefinitions>();

        public void AddDefinitions(TDefinitions def)
        {
            _definitions.Add(def);
        }

        public void Load(IReadOnlyCollection<Type> types)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TDefinition> GetDefinitions(string name)
        {
            return _definitions.SelectMany(d => d.GetDefinitions(name));
        }

        public bool TryGetDefinition(string name, int version, out TDefinition definition)
        {
            foreach (var definition1 in _definitions)
            {
                if (definition1.TryGetDefinition(name, version, out definition))
                    return true;
            }
            definition = default;
            return false;
        }

        public IEnumerable<TDefinition> GetAllDefinitions()
        {
            return _definitions.SelectMany(d => d.GetAllDefinitions());
        }

        public TDefinition GetDefinition(string name, int version)
        {
            foreach (var definition1 in _definitions)
            {
                TDefinition definition2;
                if (definition1.TryGetDefinition(name, version, out definition2))
                    return definition2;
            }
            throw new InvalidOperationException($"{(object)name} version {(object)version} not registered.");
        }

        public TDefinition GetDefinition(Type type)
        {
            foreach (var definition1 in _definitions)
            {
                TDefinition definition2;
                if (definition1.TryGetDefinition(type, out definition2))
                    return definition2;
            }
            throw new InvalidOperationException(type.Name + " not registered.");
        }

        public IReadOnlyCollection<TDefinition> GetDefinitions(Type type)
        {
            return _definitions.SelectMany(d => (IEnumerable<TDefinition>)d.GetDefinitions(type)).ToList();
        }

        public bool TryGetDefinition(Type type, out TDefinition definition)
        {
            foreach (var definition1 in _definitions)
            {
                if (definition1.TryGetDefinition(type, out definition))
                    return true;
            }
            definition = default;
            return false;
        }

        public bool TryGetDefinitions(Type type, out IReadOnlyCollection<TDefinition> definitions)
        {
            foreach (var definition in _definitions)
            {
                if (definition.TryGetDefinitions(type, out definitions))
                    return true;
            }
            definitions = null;
            return false;
        }

        public void Load(params Type[] types)
        {
            throw new NotImplementedException();
        }
    }
}
