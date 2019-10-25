using System;
using EventFly.ValueObjects;

namespace EventFly.Localization
{


    [Serializable]
    public class LocalizedImage : LocalizedValueObject<ImageLocalization>
    {
        public LocalizedImage() : this(null){}

        public LocalizedImage(params ImageLocalization[] locs) : base(locs){}

    }

    [Serializable]
    public class ImageLocalization : ValueObject, ILocalizedValue
    {
        public ImageLocalization(string src, LanguageCode lang, string alt = null)
        {
            Src = src;
            Lang = lang;
            Alt = alt;
        }

        public string Src { get; }
        public string Alt { get; }
        public LanguageCode Lang { get; }

        public override string ToString()
        {
            return $"{Src} {Lang}";
        }
    }
}