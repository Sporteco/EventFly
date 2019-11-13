namespace Demo.Predicates
{
    public sealed class DoublePredicate :  Predicate<double, double, NumericOperator>
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

    public sealed class DecimalPredicate : Predicate<decimal, decimal, NumericOperator>
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

    public sealed class IntegerPredicate : Predicate<int, int, NumericOperator>
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

    public sealed class LongPredicate : Predicate<long, long, NumericOperator>
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