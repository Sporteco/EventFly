using System;

namespace Demo.Predicates
{
    /// <summary>
    /// http://gis-lab.info/qa/great-circles.html
    /// https://en.wikipedia.org/wiki/Great-circle_distance
    /// </summary>
    public sealed class LocationPredicate : Predicate
    {
        public LocationPoint Source { get; set; }
        public DoubleRangePredicate Distance { get; set; }

        public Boolean Check(LocationPoint destination)
        {
            return Distance.Check(Source.GetDistanceTo(destination));
        }
    }

    public sealed class LocationPoint
    {
        /// <summary>
        /// Needed to correct deserialization
        /// </summary>
        public LocationPoint() { }

        public LocationPoint(Double latitude, Double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
    }

    public static class LocationPointExtension
    {
        public static Double GetDistanceTo(this LocationPoint srcPoint, LocationPoint dstPoint)
        {
            const Int64 RADIUS = 6372795;

            var lat1 = ToRadians(srcPoint.Latitude);
            var lat2 = ToRadians(dstPoint.Latitude);
            var long1 = ToRadians(srcPoint.Longitude);
            var long2 = ToRadians(dstPoint.Longitude);

            var cl1 = Math.Cos(lat1);
            var cl2 = Math.Cos(lat2);
            var sl1 = Math.Sin(lat1);
            var sl2 = Math.Sin(lat2);
            var delta = long2 - long1;
            var cdelta = Math.Cos(delta);
            var sdelta = Math.Sin(delta);

            var y = Math.Sqrt(Math.Pow(cl2 * sdelta, 2) + Math.Pow(cl1 * sl2 - sl1 * cl2 * cdelta, 2));
            var x = sl1 * sl2 + cl1 * cl2 * cdelta;
            var ad = Math.Atan2(y, x);
            return ad * RADIUS;

            Double ToRadians(Double degrees)
            {
                var radians = (Math.PI / 180) * degrees;
                return (radians);
            }
        }
    }
}