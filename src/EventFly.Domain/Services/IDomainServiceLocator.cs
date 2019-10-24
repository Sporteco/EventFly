using EventFly.Aggregates;
using EventFly.Core;

namespace EventFly.Domain.Services
{
    public interface IDomainServiceLocator<out TIdentity>
        where TIdentity : IIdentity
    {
        TIdentity LocateService(IDomainEvent domainEvent);
 
    }

    internal class EmptyIdentity : Identity<EmptyIdentity> { public EmptyIdentity(string value) : base(value){}}
}