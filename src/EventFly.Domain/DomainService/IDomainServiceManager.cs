using EventFly.Core;

namespace EventFly.Domain
{
    public interface IDomainServiceManager<TDomainService, TIdentity, TDomainServiceLocator>
        where TDomainService : IDomainService<TIdentity>
        where TIdentity : IIdentity
        where TDomainServiceLocator : class, IDomainServiceLocator<TIdentity>, new()
    { }
}