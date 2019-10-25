using System;
using EventFly.ValueObjects;

namespace EventFly.Extensions.Localization
{
    [Serializable]
    public class LocalizedLink : LocalizedValueObject<LinkLocalization>
    {
        public LocalizedLink() : this(null){}

        public LocalizedLink(params LinkLocalization[] locs) : base(locs){}

    }

    
    [Serializable]
    public class LinkLocalization : ValueObject, ILocalizedValue
    {
        public LinkLocalization(string href, LanguageCode lang, string alt = null)
        {
            Href = href;
            Lang = lang;
            Alt = alt;
        }

        public string Href { get; }
        public string Alt { get; }
        public LanguageCode Lang { get; }

        public override string ToString()
        {
            return $"{Href} {Lang}";
        }
    }
}