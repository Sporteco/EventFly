using System;
using System.Collections.Generic;
using System.Linq;
using EventFly.ValueObjects;

namespace EventFly.Extensions.Localization
{
    [Serializable]
    public class LocalizedValueObject<T> : ValueObject
        where T : ILocalizedValue
    {
        protected readonly List<T> _locs = new List<T>();

        public List<T> Locs => _locs;


        public LocalizedValueObject(params T[] locs)
        {
            _locs.AddRange(locs);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return _locs.Select(i => (object) i);
        }

    }
}
