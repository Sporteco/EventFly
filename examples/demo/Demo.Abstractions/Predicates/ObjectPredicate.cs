using System;

namespace Demo.Predicates
{
    public abstract class ObjectPredicate<TObject> : Predicate
        where TObject : class
    {
        public abstract Boolean Check(TObject @object);
    }
}
