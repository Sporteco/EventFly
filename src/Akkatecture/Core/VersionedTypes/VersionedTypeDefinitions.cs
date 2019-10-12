// Decompiled with JetBrains decompiler
// Type: Akkatecture.Core.VersionedTypes.VersionedTypeDefinitions`3
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using Akkatecture.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Akkatecture.Core.VersionedTypes
{
  public abstract class VersionedTypeDefinitions<TTypeCheck, TAttribute, TDefinition> : IVersionedTypeDefinitions<TAttribute, TDefinition>
    where TAttribute : VersionedTypeAttribute
    where TDefinition : VersionedTypeDefinition
  {
    private static readonly Regex NameRegex = new Regex("^(Old){0,1}(?<name>[\\p{L}\\p{Nd}]+?)(V(?<version>[0-9]+)){0,1}$", RegexOptions.Compiled);
    private readonly object _syncRoot = new object();
    private readonly ConcurrentDictionary<Type, List<TDefinition>> _definitionsByType = new ConcurrentDictionary<Type, List<TDefinition>>();
    private readonly ConcurrentDictionary<string, Dictionary<int, TDefinition>> _definitionByNameAndVersion = new ConcurrentDictionary<string, Dictionary<int, TDefinition>>();

    public void Load(params Type[] types)
    {
      this.Load((IReadOnlyCollection<Type>) types);
    }

    public void Load(IReadOnlyCollection<Type> types)
    {
      if (types == null)
        return;
      List<Type> list1 = types.Where<Type>((Func<Type, bool>) (t => !typeof (TTypeCheck).GetTypeInfo().IsAssignableFrom(t))).ToList<Type>();
      if (list1.Any<Type>())
        throw new ArgumentException("The following types are not of type '" + typeof (TTypeCheck).PrettyPrint() + "': " + string.Join(", ", list1.Select<Type, string>((Func<Type, string>) (t => t.PrettyPrint()))));
      lock (this._syncRoot)
      {
        List<TDefinition> list2 = types.Distinct<Type>().Where<Type>((Func<Type, bool>) (t => !this._definitionsByType.ContainsKey(t))).SelectMany<Type, TDefinition>(new Func<Type, IEnumerable<TDefinition>>(this.CreateDefinitions)).ToList<TDefinition>();
        if (!list2.Any<TDefinition>())
          return;
        foreach (TDefinition definition in list2)
        {
          this._definitionsByType.GetOrAdd(definition.Type, (Func<Type, List<TDefinition>>) (_ => new List<TDefinition>())).Add(definition);
          Dictionary<int, TDefinition> dictionary;
          if (!this._definitionByNameAndVersion.TryGetValue(definition.Name, out dictionary))
          {
            dictionary = new Dictionary<int, TDefinition>();
            this._definitionByNameAndVersion.TryAdd(definition.Name, dictionary);
          }
          if (!dictionary.ContainsKey(definition.Version))
            dictionary.Add(definition.Version, definition);
        }
      }
    }

    public IEnumerable<TDefinition> GetDefinitions(string name)
    {
      Dictionary<int, TDefinition> dictionary;
      return this._definitionByNameAndVersion.TryGetValue(name, out dictionary) ? (IEnumerable<TDefinition>) dictionary.Values.OrderBy<TDefinition, int>((Func<TDefinition, int>) (d => d.Version)) : Enumerable.Empty<TDefinition>();
    }

    public IEnumerable<TDefinition> GetAllDefinitions()
    {
      return this._definitionByNameAndVersion.SelectMany<KeyValuePair<string, Dictionary<int, TDefinition>>, TDefinition>((Func<KeyValuePair<string, Dictionary<int, TDefinition>>, IEnumerable<TDefinition>>) (kv => (IEnumerable<TDefinition>) kv.Value.Values));
    }

    public bool TryGetDefinition(string name, int version, out TDefinition definition)
    {
      Dictionary<int, TDefinition> dictionary;
      if (this._definitionByNameAndVersion.TryGetValue(name, out dictionary))
        return dictionary.TryGetValue(version, out definition);
      definition = default (TDefinition);
      return false;
    }

    public TDefinition GetDefinition(string name, int version)
    {
      TDefinition definition;
      if (!this.TryGetDefinition(name, version, out definition))
        throw new ArgumentException(string.Format("No versioned type definition for '{0}' with version {1} in '{2}'", (object) name, (object) version, (object) this.GetType().PrettyPrint()));
      return definition;
    }

    public TDefinition GetDefinition(Type type)
    {
      TDefinition definition;
      if (!this.TryGetDefinition(type, out definition))
        throw new ArgumentException("No definition for type '" + type.PrettyPrint() + "', have you remembered to load it during Akkatecture initialization");
      return definition;
    }

    public IReadOnlyCollection<TDefinition> GetDefinitions(Type type)
    {
      IReadOnlyCollection<TDefinition> definitions;
      if (!this.TryGetDefinitions(type, out definitions))
        throw new ArgumentException("No definition for type '" + type.PrettyPrint() + "', have you remembered to load it during Akkatecture initialization");
      return definitions;
    }

    public bool TryGetDefinition(Type type, out TDefinition definition)
    {
      IReadOnlyCollection<TDefinition> definitions;
      if (!this.TryGetDefinitions(type, out definitions))
      {
        definition = default (TDefinition);
        return false;
      }
      if (definitions.Count > 1)
        throw new InvalidOperationException("Type '" + type.PrettyPrint() + "' has multiple definitions: " + string.Join(", ", definitions.Select<TDefinition, string>((Func<TDefinition, string>) (d => d.ToString()))));
      definition = definitions.Single<TDefinition>();
      return true;
    }

    public bool TryGetDefinitions(Type type, out IReadOnlyCollection<TDefinition> definitions)
    {
      if (type == (Type) null)
        throw new ArgumentNullException(nameof (type));
      List<TDefinition> definitionList;
      if (!this._definitionsByType.TryGetValue(type, out definitionList))
      {
        definitions = (IReadOnlyCollection<TDefinition>) null;
        return false;
      }
      definitions = (IReadOnlyCollection<TDefinition>) definitionList;
      return true;
    }

    protected abstract TDefinition CreateDefinition(int version, Type type, string name);

    private IEnumerable<TDefinition> CreateDefinitions(Type versionedType)
    {
      bool hasAttributeDefinition = false;
      foreach (TDefinition definition in this.CreateDefinitionFromAttribute(versionedType))
      {
        TDefinition definitionFromAttribute = definition;
        hasAttributeDefinition = true;
        yield return definitionFromAttribute;
        definitionFromAttribute = default (TDefinition);
      }
      if (!hasAttributeDefinition)
        yield return this.CreateDefinitionFromName(versionedType);
    }

    private TDefinition CreateDefinitionFromName(Type versionedType)
    {
      Match match = VersionedTypeDefinitions<TTypeCheck, TAttribute, TDefinition>.NameRegex.Match(versionedType.Name);
      if (!match.Success)
        throw new ArgumentException("Versioned type name '" + versionedType.Name + "' is not a valid name");
      int version = 1;
      Group group = match.Groups["version"];
      if (group.Success)
        version = int.Parse(group.Value);
      string name = match.Groups["name"].Value;
      return this.CreateDefinition(version, versionedType, name);
    }

    private IEnumerable<TDefinition> CreateDefinitionFromAttribute(Type versionedType)
    {
      return versionedType.GetTypeInfo().GetCustomAttributes().OfType<TAttribute>().Select<TAttribute, TDefinition>((Func<TAttribute, TDefinition>) (a => this.CreateDefinition(a.Version, versionedType, a.Name)));
    }
  }
}
