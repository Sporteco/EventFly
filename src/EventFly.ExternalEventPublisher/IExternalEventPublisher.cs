using EventFly.Aggregates;
using System.Threading.Tasks;

namespace EventFly.ExternalEventPublisher
{
    public interface IExternalEventPublisher
    {
        Task Publish(IDomainEvent domainEvent);
    }
}
