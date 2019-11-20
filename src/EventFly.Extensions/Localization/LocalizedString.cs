using EventFly.ValueObjects;
using System;
using System.Linq;

namespace EventFly.Localization
{
    [Serializable]
    public class LocalizedString : LocalizedValueObject<StringLocalization>
    {
        public LocalizedString() : this(null) { }

        public LocalizedString(params StringLocalization[] locs) : base(locs) { }

        public static implicit operator LocalizedString((String value, LanguageCode lang) str)
        {
            return new LocalizedString(str);
        }

        public override String ToString()
        {
            return String.Join(Environment.NewLine, Locs.Select(i => $"{i.Value} {i.Lang}"));
        }
    }

    [Serializable]
    public class StringLocalization : ValueObject, ILocalizedValue
    {
        public String Value { get; }
        public LanguageCode Lang { get; }

        public StringLocalization(String value, LanguageCode lang)
        {
            Value = value;
            Lang = lang;
        }

        public static implicit operator StringLocalization((String value, LanguageCode lang) str)
        {
            return new StringLocalization(str.value, str.lang);
        }

        public override String ToString() => $"{Value} {Lang}";
    }
}