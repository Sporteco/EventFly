using EventFly.ValueObjects;
using System;

namespace EventFly.Localization
{


    [Serializable]
    public class LocalizedImage : LocalizedValueObject<ImageLocalization>
    {
        public LocalizedImage() : this(null) { }

        public LocalizedImage(params ImageLocalization[] locs) : base(locs) { }

    }

    [Serializable]
    public class ImageLocalization : ValueObject, ILocalizedValue
    {
        public ImageLocalization(String src, LanguageCode lang, String alt = null)
        {
            Src = src;
            Lang = lang;
            Alt = alt;
        }

        public String Src { get; }
        public String Alt { get; }
        public LanguageCode Lang { get; }

        public override String ToString()
        {
            return $"{Src} {Lang}";
        }
    }
}