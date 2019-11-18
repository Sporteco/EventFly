using System;

namespace Demo.Predicates
{
    public abstract class RangePredicate<TRange, TValue> : Predicate
    {
        public TRange From { get; set; }
        public TRange To { get; set; }

        public abstract Boolean Check(TValue value);
    }
}