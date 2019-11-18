using System;
using System.Linq;

namespace EventFly.Localization
{
    [Serializable]
    public class LocalizedMarkdownString : LocalizedValueObject<StringLocalization>
    {
        public LocalizedMarkdownString() : this(null) { }

        public LocalizedMarkdownString(params StringLocalization[] locs) : base(locs)
        {
        }

        public static implicit operator LocalizedMarkdownString((String value, LanguageCode lang) str)
        {
            return new LocalizedMarkdownString(str);
        }

        public override String ToString()
        {
            return String.Join(Environment.NewLine, Locs.Select(i => $"{i.Value} {i.Lang}"));
        }
    }
}