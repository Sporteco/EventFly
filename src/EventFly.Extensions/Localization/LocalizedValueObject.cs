using System;
using System.Collections.Generic;
using System.Linq;
using EventFly.ValueObjects;
using Newtonsoft.Json;

namespace EventFly.Localization
{
    [Serializable]
    public class LocalizedValueObject<T> : ValueObject
        where T : ILocalizedValue
    {
        protected readonly List<T> _locs = new List<T>();

        public T[] Locs => _locs.ToArray();

        [JsonConstructor]
        public LocalizedValueObject(params T[] locs)
        {
            if (locs != null)
                _locs.AddRange(locs);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return _locs.Select(i => (object) i);
        }

    }
}
