using EventFly.Aggregates;
using EventFly.Jobs;
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

    internal sealed class JobEventsScheduler : IEventsScheduler
    {
        private readonly IScheduler _scheduler;

        public JobEventsScheduler(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public Task<bool> Schedule(PublishEventJobId id, IDomainEvent @event, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishEventJob, PublishEventJobId>(new PublishEventJob(id, @event), triggerDate);
        }

        public Task<bool> Schedule(PublishEventJobId id, IDomainEvent @event, TimeSpan interval, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishEventJob, PublishEventJobId>(new PublishEventJob(id, @event), interval, triggerDate);
        }

        public Task<bool> Schedule(PublishEventJobId id, IDomainEvent @event, string cronExpression, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishEventJob, PublishEventJobId>(new PublishEventJob(id, @event), cronExpression, triggerDate);
        }

        public Task<bool> Cancel(PublishEventJobId jobId)
        {
            return _scheduler.Cancel<PublishEventJob, PublishEventJobId>(jobId);
        }
    }
}
