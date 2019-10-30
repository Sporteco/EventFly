namespace EventFly.Domain
{
    public interface IDomainServiceManager<TDomainService>
        where TDomainService : IDomainService
    { }
}