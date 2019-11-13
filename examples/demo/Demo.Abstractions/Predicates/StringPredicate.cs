using System.Collections.Generic;
using System.Linq;

namespace Demo.Predicates
{
    public sealed class StringPredicate : Predicate<string, IEnumerable<string>, StringOperator>
    {
        public StringPredicate()
        {
            Register(StringOperator.Equals, (left, right) => 
                right is null 
                || right.All(s => s is null || left.ToLowerInvariant().Equals(s.ToLowerInvariant())));
            Register(StringOperator.Contains, (left, right) => 
                right is null ||
                right.All(s => s is null || left.ToLowerInvariant().Contains(s.ToLowerInvariant())));
            Register(StringOperator.StartsWith, (left, right) => 
                right is null || 
                right.All(s => s is null || left.ToLowerInvariant().StartsWith(s.ToLowerInvariant())));
            Register(StringOperator.EndsWith, (left, right) => 
                right is null
                || right.All(s => s is null || left.ToLowerInvariant().EndsWith(s.ToLowerInvariant())));
            Register(StringOperator.OneOf, (left, right) => 
                right is null || 
                right.Any(s => s is null || left.ToLowerInvariant().Equals(s.ToLowerInvariant())));
        }
    }

    public enum StringOperator
    {
        Equals,
        Contains,
        StartsWith,
        EndsWith,
        OneOf
    }
}
