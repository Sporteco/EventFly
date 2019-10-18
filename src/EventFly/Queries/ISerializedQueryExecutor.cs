using System.Threading;
using System.Threading.Tasks;

namespace EventFly.Queries
{
    public interface ISerializedQueryExecutor
    {
        Task<object> ExecuteQueryAsync(
            string name,
            string json,
            CancellationToken cancellationToken);
    }
}