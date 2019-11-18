using EventFly.ValueObjects;
using System;
using System.Globalization;
using System.Linq;

namespace EventFly.Localization
{
    [Serializable]
    public class LanguageCode : SingleValueObject<String>
    {
        public LanguageCode(String value) : base(value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (!CultureInfo.GetCultures(CultureTypes.AllCultures).Any(c => c.Name.Equals(Value)))
                throw new InvalidOperationException($"Invalid culture name: {Value}");
        }

        public static implicit operator LanguageCode(String lang)
        {
            return new LanguageCode(lang.ToLower());
        }

        public override String ToString()
        {
            return $"[{Value}]";
        }
    }
}