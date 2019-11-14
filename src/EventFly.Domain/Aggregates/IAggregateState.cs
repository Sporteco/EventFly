using EventFly.Core;
using System.Threading.Tasks;

namespace EventFly.Aggregates
{
    public interface IAggregateState<TIdentity>
        where TIdentity : IIdentity
    {
        TIdentity Id { get; set; }

        Task LoadState(TIdentity id);

        Task UpdateAndSaveState(IAggregateEvent<TIdentity> @event);
    }
}