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

        public bool Check(LocationPoint destination)
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

        public LocationPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public static class LocationPointExtension
    {
        public static double GetDistanceTo(this LocationPoint srcPoint, LocationPoint dstPoint)
        {
            const long RADIUS = 6372795;

            double lat1 = ToRadians(srcPoint.Latitude);
            double lat2 = ToRadians(dstPoint.Latitude);
            double long1 = ToRadians(srcPoint.Longitude);
            double long2 = ToRadians(dstPoint.Longitude);

            double cl1 = Math.Cos(lat1);
            double cl2 = Math.Cos(lat2);
            double sl1 = Math.Sin(lat1);
            double sl2 = Math.Sin(lat2);
            double delta = long2 - long1;
            double cdelta = Math.Cos(delta);
            double sdelta = Math.Sin(delta);

            double y = Math.Sqrt(Math.Pow(cl2 * sdelta, 2) + Math.Pow(cl1 * sl2 - sl1 * cl2 * cdelta, 2));
            double x = sl1 * sl2 + cl1 * cl2 * cdelta;
            double ad = Math.Atan2(y, x);
            return ad * RADIUS;

            double ToRadians(double degrees)
            {
                double radians = (Math.PI / 180) * degrees;
                return (radians);
            }
        }
    }
}