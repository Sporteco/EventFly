using EventFly.Aggregates;
using EventFly.Schedulers.Events;
using System;
using System.Threading.Tasks;

namespace EventFly.Schedulers
{
    public interface IEventsScheduler
    {
        Task<bool> Schedule(PublishEventJobId id, IDomainEvent @event, DateTime triggerDate);

        Task<bool> Schedule(PublishEventJobId id, IDomainEvent @event, TimeSpan interval, DateTime triggerDate);

        Task<bool> Schedule(PublishEventJobId id, IDomainEvent @event, string cronExpression, DateTime triggerDate);

        Task<bool> Cancel(PublishEventJobId jobId);
    }
}
