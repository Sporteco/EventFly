using System;
using System.Globalization;
using System.Linq;
using EventFly.ValueObjects;

namespace EventFly.Extensions.Localization
{
    [Serializable]
    public class LanguageCode : SingleValueObject<string>
    {
        public LanguageCode(string value) : base(value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (!CultureInfo.GetCultures(CultureTypes.AllCultures).Any(c => c.Name.Equals(Value)))
                throw new InvalidOperationException($"Invalid culture name: {Value}");
        }

        public static implicit operator LanguageCode(string lang)
        {
            return new LanguageCode(lang.ToLower());
        }

        public override string ToString()
        {
            return $"[{Value}]";
        }
    }
}