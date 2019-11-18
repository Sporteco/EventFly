using System.Threading.Tasks;

namespace EventFly.Queries
{
    public interface IQueryProcessor
    {
        Task<TResult> Process<TResult>(IQuery<TResult> query);
        Task<System.Object> Process(IQuery query);
    }
}
