using EventFly.Aggregates;
using EventFly.Jobs;
using EventFly.Schedulers.Events;
using System;
using System.Threading.Tasks;

namespace EventFly.Schedulers
{
    internal sealed class JobEventsScheduler : IEventsScheduler
    {
        private readonly IScheduler _scheduler;

        public JobEventsScheduler(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public Task<Boolean> Schedule(PublishEventJobId id, IDomainEvent @event, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishEventJob, PublishEventJobId>(new PublishEventJob(id, @event), triggerDate);
        }

        public Task<Boolean> Schedule(PublishEventJobId id, IDomainEvent @event, TimeSpan interval, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishEventJob, PublishEventJobId>(new PublishEventJob(id, @event), interval, triggerDate);
        }

        public Task<Boolean> Schedule(PublishEventJobId id, IDomainEvent @event, String cronExpression, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishEventJob, PublishEventJobId>(new PublishEventJob(id, @event), cronExpression, triggerDate);
        }

        public Task<Boolean> Cancel(PublishEventJobId jobId)
        {
            return _scheduler.Cancel<PublishEventJob, PublishEventJobId>(jobId);
        }
    }
}
