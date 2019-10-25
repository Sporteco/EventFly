using System;
using System.Linq;

namespace EventFly.Extensions.Localization
{
    [Serializable]
    public class LocalizedMarkdownString : LocalizedValueObject<StringLocalization>
    {
        public LocalizedMarkdownString() : this(null){}

        public LocalizedMarkdownString(params StringLocalization[] locs) : base(locs)
        {
        }

        public static implicit operator LocalizedMarkdownString((string value, LanguageCode lang) str)
        {
            return new LocalizedMarkdownString(str);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Locs.Select(i => $"{i.Value} {i.Lang}"));
        }
    }
}