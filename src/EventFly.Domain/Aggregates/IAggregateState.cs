using System.Threading.Tasks;
using EventFly.Core;

namespace EventFly.Aggregates
{
    public interface IAggregateState<TIdentity> 
        where TIdentity : IIdentity
    {
        TIdentity Id { get; set; }

        Task LoadState(TIdentity id);
    }
}