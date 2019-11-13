namespace Demo.Predicates
{
    public sealed class DoubleRangePredicate : RangePredicate<double?, double>
    {
        public override bool Check(double value)
        {
            return value >= (From ?? value) && value <= (To ?? value);
        }
    }

    public sealed class DecimalRangePredicate : RangePredicate<decimal?, decimal>
    {
        public override bool Check(decimal value)
        {
            return value >= (From ?? value) && value <= (To ?? value);
        }
    }

    public sealed class IntegerRangePredicate : RangePredicate<int?, int>
    {
        public override bool Check(int value)
        {
            return value >= (From ?? value) && value <= (To ?? value);
        }
    }

    public sealed class LongRangePredicate : RangePredicate<long?, long>
    {
        public override bool Check(long value)
        {
            return value >= (From ?? value) && value <= (To ?? value);
        }
    }
}
