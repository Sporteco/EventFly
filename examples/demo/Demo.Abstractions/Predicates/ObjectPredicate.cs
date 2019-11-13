namespace Demo.Predicates
{
    public abstract class ObjectPredicate<TObject> : Predicate
        where TObject : class
    {
        public abstract bool Check(TObject @object);
    }
}
