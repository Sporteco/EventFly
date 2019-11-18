using System;

namespace Demo.Predicates
{
    public sealed class DoubleRangePredicate : RangePredicate<Double?, Double>
    {
        public override Boolean Check(Double value)
        {
            return value >= (From ?? value) && value <= (To ?? value);
        }
    }

    public sealed class DecimalRangePredicate : RangePredicate<Decimal?, Decimal>
    {
        public override Boolean Check(Decimal value)
        {
            return value >= (From ?? value) && value <= (To ?? value);
        }
    }

    public sealed class IntegerRangePredicate : RangePredicate<Int32?, Int32>
    {
        public override Boolean Check(Int32 value)
        {
            return value >= (From ?? value) && value <= (To ?? value);
        }
    }

    public sealed class LongRangePredicate : RangePredicate<Int64?, Int64>
    {
        public override Boolean Check(Int64 value)
        {
            return value >= (From ?? value) && value <= (To ?? value);
        }
    }
}