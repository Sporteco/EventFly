using System;
using System.Linq;
using EventFly.ValueObjects;

namespace EventFly.Localization
{

    [Serializable]
    public class LocalizedString : LocalizedValueObject<StringLocalization>
    {
        public LocalizedString() : this(null){}

        public LocalizedString(params StringLocalization[] locs) : base(locs){}

        public static implicit operator LocalizedString((string value, LanguageCode lang) str)
        {
            return new LocalizedString(str);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Locs.Select(i => $"{i.Value} {i.Lang}"));
        }
    }
    [Serializable]
    public class StringLocalization : ValueObject, ILocalizedValue
    {
        public StringLocalization(string value, LanguageCode lang)
        {
            Value = value;
            Lang = lang;
        }

        public string Value { get; }
        public LanguageCode Lang { get; }

        public static implicit operator StringLocalization((string value, LanguageCode lang) str)
        {
            return new StringLocalization(str.value, str.lang);
        }

        public override string ToString()
        {
            return $"{Value} {Lang}";
        }
    }
}