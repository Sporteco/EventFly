using Akkatecture.Core;

namespace Akkatecture.Queries
{
    public interface IAggregateQueryHandler<TIdentity, in TQuery, TQueryResult>
        where TIdentity : IIdentity
        where TQuery :  IAggregateQuery<TIdentity, TQueryResult>
    {
        TQueryResult ExecuteQuery(TQuery query);
    }
}