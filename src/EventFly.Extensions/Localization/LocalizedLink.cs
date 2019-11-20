using EventFly.ValueObjects;
using System;

namespace EventFly.Localization
{
    [Serializable]
    public class LocalizedLink : LocalizedValueObject<LinkLocalization>
    {
        public LocalizedLink() : this(null) { }

        public LocalizedLink(params LinkLocalization[] locs) : base(locs) { }
    }

    [Serializable]
    public class LinkLocalization : ValueObject, ILocalizedValue
    {
        public String Href { get; }
        public String Alt { get; }
        public LanguageCode Lang { get; }

        public LinkLocalization(String href, LanguageCode lang, String alt = null)
        {
            Href = href;
            Lang = lang;
            Alt = alt;
        }

        public override String ToString() => $"{Href} {Lang}";
    }
}