
using Akkatecture.Core;

namespace Akkatecture.Queries
{
    public interface IAggregateQuery<out TIdentity, TResult> : IQuery<TResult>
        where TIdentity : IIdentity
    {
        TIdentity Id { get; }
    }

    public abstract class AggregateQuery<TIdentity, TResult> : IAggregateQuery<TIdentity, TResult> where TIdentity : IIdentity
    {
        public TIdentity Id { get; }
        protected AggregateQuery(TIdentity id) => Id = id;

    }
}
