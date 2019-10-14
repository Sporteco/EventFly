using System;

namespace Demo.Predicates
{
    public sealed class DateTimePredicate : Predicate<DateTime, DateTime, DateTimeOperator>
    {
        public DateTimePredicate()
        {
            Register(DateTimeOperator.Equals, (left, right) => left == right);
            Register(DateTimeOperator.Less, (left, right) => left < right);
            Register(DateTimeOperator.LessOrEquals, (left, right) => left <= right);
            Register(DateTimeOperator.More, (left, right) => left > right);
            Register(DateTimeOperator.MoreOrEquals, (left, right) => left >= right);
        }
    }

    public enum DateTimeOperator
    {
        Equals,
        Less,
        LessOrEquals,
        More,
        MoreOrEquals
    }
}