using System.Collections.Generic;
using System.Linq;

namespace Demo.Predicates
{
    public abstract class CollectionPredicate<TObject, TObjectPredicate> : Predicate<IEnumerable<TObject>, TObjectPredicate, CollectionOperator>
        where TObject : class
        where TObjectPredicate : ObjectPredicate<TObject>
    {
        public CollectionPredicate()
        {
            Register(CollectionOperator.Any, (left, right) => left.Any(e => right.Check(e)));
            Register(CollectionOperator.All, (left, right) => left.All(e => right.Check(e)));
        }
    }

    public enum CollectionOperator
    {
        Any,
        All
    }
}
