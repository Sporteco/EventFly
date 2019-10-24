using System.Threading.Tasks;

namespace EventFly.Queries
{

    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> ExecuteQuery(TQuery query);
    }
}
