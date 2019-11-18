using System.Threading;
using System.Threading.Tasks;

namespace EventFly.Queries
{
    public interface ISerializedQueryExecutor
    {
        Task<System.Object> ExecuteQueryAsync(
            System.String name,
            System.String json,
            CancellationToken cancellationToken);
    }
}