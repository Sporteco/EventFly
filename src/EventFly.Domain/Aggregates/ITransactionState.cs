using System.Threading.Tasks;

namespace EventFly.Aggregates
{
    public interface ITransactionState
    {
        Task BeginTransaction();
        Task CommitTransaction();
        Task RollbackTransaction();

    }
}