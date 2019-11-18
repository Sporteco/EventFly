using System;

namespace Demo.Predicates
{
    public sealed class DateTimeRangePredicate : RangePredicate<DateTime?, DateTime>
    {
        public override Boolean Check(DateTime value)
        {
            return value >= (From ?? value) && value <= (To ?? value);
        }
    }
}