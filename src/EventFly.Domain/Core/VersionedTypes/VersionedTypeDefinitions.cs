// Decompiled with JetBrains decompiler
// Type: EventFly.Core.VersionedTypes.VersionedTypeDefinitions`3
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using EventFly.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EventFly.Core.VersionedTypes
{
    public abstract class VersionedTypeDefinitions<TTypeCheck, TAttribute, TDefinition> : IVersionedTypeDefinitions<TAttribute, TDefinition>
      where TAttribute : VersionedTypeAttribute
      where TDefinition : VersionedTypeDefinition
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Regex NameRegex = new Regex("^(Old){0,1}(?<name>[\\p{L}\\p{Nd}]+?)(V(?<version>[0-9]+)){0,1}$", RegexOptions.Compiled);
        private readonly Object _syncRoot = new Object();
        private readonly ConcurrentDictionary<Type, List<TDefinition>> _definitionsByType = new ConcurrentDictionary<Type, List<TDefinition>>();
        private readonly ConcurrentDictionary<String, Dictionary<Int32, TDefinition>> _definitionByNameAndVersion = new ConcurrentDictionary<String, Dictionary<Int32, TDefinition>>();

        public void Load(params Type[] types)
        {
            Load((IReadOnlyCollection<Type>)types);
        }

        public void Load(IReadOnlyCollection<Type> types)
        {
            if (types == null)
                return;
            var list1 = types.Where(t => !typeof(TTypeCheck).GetTypeInfo().IsAssignableFrom((TypeInfo)t)).ToList();
            if (list1.Any())
                throw new ArgumentException("The following types are not of type '" + typeof(TTypeCheck).PrettyPrint() + "': " + String.Join(", ", list1.Select(t => t.PrettyPrint())));
            lock (_syncRoot)
            {
                var list2 = types.Distinct().Where(t => !_definitionsByType.ContainsKey(t)).SelectMany(CreateDefinitions).ToList();
                if (!list2.Any())
                    return;
                foreach (var definition in list2)
                {
                    _definitionsByType.GetOrAdd(definition.Type, _ => new List<TDefinition>()).Add(definition);
                    Dictionary<Int32, TDefinition> dictionary;
                    if (!_definitionByNameAndVersion.TryGetValue(definition.Name, out dictionary))
                    {
                        dictionary = new Dictionary<Int32, TDefinition>();
                        _definitionByNameAndVersion.TryAdd(definition.Name, dictionary);
                    }
                    if (!dictionary.ContainsKey(definition.Version))
                        dictionary.Add(definition.Version, definition);
                }
            }
        }

        public IEnumerable<TDefinition> GetDefinitions(String name)
        {
            Dictionary<Int32, TDefinition> dictionary;
            return _definitionByNameAndVersion.TryGetValue(name, out dictionary) ? dictionary.Values.OrderBy(d => d.Version) : Enumerable.Empty<TDefinition>();
        }

        public IEnumerable<TDefinition> GetAllDefinitions()
        {
            return _definitionByNameAndVersion.SelectMany(kv => (IEnumerable<TDefinition>)kv.Value.Values);
        }

        public Boolean TryGetDefinition(String name, Int32 version, out TDefinition definition)
        {
            Dictionary<Int32, TDefinition> dictionary;
            if (_definitionByNameAndVersion.TryGetValue(name, out dictionary))
                return dictionary.TryGetValue(version, out definition);
            definition = default;
            return false;
        }

        public TDefinition GetDefinition(String name, Int32 version)
        {
            TDefinition definition;
            if (!TryGetDefinition(name, version, out definition))
                throw new ArgumentException(
                    $"No versioned type definition for '{(Object)name}' with version {(Object)version} in '{(Object)GetType().PrettyPrint()}'");
            return definition;
        }

        public TDefinition GetDefinition(Type type)
        {
            TDefinition definition;
            if (!TryGetDefinition(type, out definition))
                throw new ArgumentException("No definition for type '" + type.PrettyPrint() + "', have you remembered to load it during EventFly initialization");
            return definition;
        }

        public IReadOnlyCollection<TDefinition> GetDefinitions(Type type)
        {
            IReadOnlyCollection<TDefinition> definitions;
            if (!TryGetDefinitions(type, out definitions))
                throw new ArgumentException("No definition for type '" + type.PrettyPrint() + "', have you remembered to load it during EventFly initialization");
            return definitions;
        }

        public Boolean TryGetDefinition(Type type, out TDefinition definition)
        {
            IReadOnlyCollection<TDefinition> definitions;
            if (!TryGetDefinitions(type, out definitions))
            {
                definition = default;
                return false;
            }
            if (definitions.Count > 1)
                throw new InvalidOperationException("Type '" + type.PrettyPrint() + "' has multiple definitions: " + String.Join(", ", definitions.Select(d => d.ToString())));
            definition = definitions.Single();
            return true;
        }

        public Boolean TryGetDefinitions(Type type, out IReadOnlyCollection<TDefinition> definitions)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            List<TDefinition> definitionList;
            if (!_definitionsByType.TryGetValue(type, out definitionList))
            {
                definitions = null;
                return false;
            }
            definitions = definitionList;
            return true;
        }

        protected abstract TDefinition CreateDefinition(Int32 version, Type type, String name);

        private IEnumerable<TDefinition> CreateDefinitions(Type versionedType)
        {
            var hasAttributeDefinition = false;
            foreach (var definition in CreateDefinitionFromAttribute(versionedType))
            {
                var definitionFromAttribute = definition;
                hasAttributeDefinition = true;
                yield return definitionFromAttribute;
            }
            if (!hasAttributeDefinition)
                yield return CreateDefinitionFromName(versionedType);
        }

        private TDefinition CreateDefinitionFromName(Type versionedType)
        {
            var match = NameRegex.Match(versionedType.Name);
            if (!match.Success)
                throw new ArgumentException("Versioned type name '" + versionedType.Name + "' is not a valid name");
            var version = 1;
            var group = match.Groups["version"];
            if (group.Success)
                version = Int32.Parse(group.Value);
            var name = match.Groups["name"].Value;
            return CreateDefinition(version, versionedType, name);
        }

        private IEnumerable<TDefinition> CreateDefinitionFromAttribute(Type versionedType)
        {
            return versionedType.GetTypeInfo().GetCustomAttributes().OfType<TAttribute>().Select(a => CreateDefinition(a.Version, versionedType, a.Name));
        }
    }
}
