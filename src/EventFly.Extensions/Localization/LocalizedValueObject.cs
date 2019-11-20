using EventFly.ValueObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventFly.Localization
{
    [Serializable]
    public class LocalizedValueObject<T> : ValueObject
        where T : ILocalizedValue
    {
        public T[] Locs => _locs.ToArray();

        [JsonConstructor]
        public LocalizedValueObject(params T[] locs)
        {
            if (locs != null) _locs.AddRange(locs);
        }

        protected readonly List<T> _locs = new List<T>();

        protected override IEnumerable<Object> GetEqualityComponents()
        {
            return _locs.Select(i => (Object)i);
        }
    }
}