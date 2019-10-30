namespace EventFly.DomainService
{
    public interface IDomainServiceManager<TDomainService>
        where TDomainService : IDomainService
    { }
}