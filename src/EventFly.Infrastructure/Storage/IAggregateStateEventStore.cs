using EventFly.Aggregates;
using EventFly.Core;
using System.Threading.Tasks;

namespace EventFly.Infrastructure.Storage
{
    public interface IAggregateStateEventStore
    {
        Task<IAggregateEvent<T>[]> GetEvents<T>(T id) where T : IIdentity;
        Task Put<T>(T id, IAggregateEvent<T> @event) where T : IIdentity;
    }
}