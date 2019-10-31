using EventFly.Commands;

namespace EventFly.DomainService
{
    public abstract class BaseDomainService<TDomainService>
        where TDomainService : BaseDomainService<TDomainService>, new()
    {
        protected ICommandBus CommandBus { get; private set; }

        internal void Inject(ICommandBus commandBus)
        {
            CommandBus = commandBus;
        }
    }
}
