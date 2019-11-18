using System;

namespace Demo.Predicates
{
    public sealed class DoublePredicate : Predicate<Double, Double, NumericOperator>
    {
        public DoublePredicate()
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            Register(NumericOperator.Equals, (left, right) => left == right);
            Register(NumericOperator.Less, (left, right) => left < right);
            Register(NumericOperator.LessOrEquals, (left, right) => left <= right);
            Register(NumericOperator.More, (left, right) => left > right);
            Register(NumericOperator.MoreOrEquals, (left, right) => left >= right);
        }
    }

    public sealed class DecimalPredicate : Predicate<Decimal, Decimal, NumericOperator>
    {
        public DecimalPredicate()
        {
            Register(NumericOperator.Equals, (left, right) => left == right);
            Register(NumericOperator.Less, (left, right) => left < right);
            Register(NumericOperator.LessOrEquals, (left, right) => left <= right);
            Register(NumericOperator.More, (left, right) => left > right);
            Register(NumericOperator.MoreOrEquals, (left, right) => left >= right);
        }
    }

    public sealed class IntegerPredicate : Predicate<Int32, Int32, NumericOperator>
    {
        public IntegerPredicate()
        {
            Register(NumericOperator.Equals, (left, right) => left == right);
            Register(NumericOperator.Less, (left, right) => left < right);
            Register(NumericOperator.LessOrEquals, (left, right) => left <= right);
            Register(NumericOperator.More, (left, right) => left > right);
            Register(NumericOperator.MoreOrEquals, (left, right) => left >= right);
        }
    }

    public sealed class LongPredicate : Predicate<Int64, Int64, NumericOperator>
    {
        public LongPredicate()
        {
            Register(NumericOperator.Equals, (left, right) => left == right);
            Register(NumericOperator.Less, (left, right) => left < right);
            Register(NumericOperator.LessOrEquals, (left, right) => left <= right);
            Register(NumericOperator.More, (left, right) => left > right);
            Register(NumericOperator.MoreOrEquals, (left, right) => left >= right);
        }
    }

    public enum NumericOperator
    {
        Equals,
        Less,
        LessOrEquals,
        More,
        MoreOrEquals
    }
}