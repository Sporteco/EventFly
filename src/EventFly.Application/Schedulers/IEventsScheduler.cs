using EventFly.Aggregates;
using EventFly.Schedulers.Events;
using System;
using System.Threading.Tasks;

namespace EventFly.Schedulers
{
    public interface IEventsScheduler
    {
        Task<Boolean> Schedule(PublishEventJobId id, IDomainEvent @event, DateTime triggerDate);

        Task<Boolean> Schedule(PublishEventJobId id, IDomainEvent @event, TimeSpan interval, DateTime triggerDate);

        Task<Boolean> Schedule(PublishEventJobId id, IDomainEvent @event, String cronExpression, DateTime triggerDate);

        Task<Boolean> Cancel(PublishEventJobId jobId);
    }
}
