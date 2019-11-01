using EventFly.Commands;

namespace EventFly.DomainService
{
    public abstract class SynchronizedDomainService<TDomainService>
        where TDomainService : SynchronizedDomainService<TDomainService>, new()
    {
        protected ICommandBus CommandBus { get; private set; }

        internal void Inject(ICommandBus commandBus)
        {
            CommandBus = commandBus;
        }
    }
}
