namespace EventFly.Queries
{
    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        TResult ExecuteQuery(TQuery query);
    }
}
