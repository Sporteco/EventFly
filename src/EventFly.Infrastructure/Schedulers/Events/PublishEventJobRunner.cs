using EventFly.Infrastructure.Jobs;
using EventFly.Jobs;
using EventFly.Schedulers.Events;

namespace EventFly.Infrastructure.Schedulers.Events
{
    public sealed class PublishEventJobRunner : JobRunner<PublishEventJob, PublishEventJobId>,
        IRun<PublishEventJob, PublishEventJobId>

    {
        public System.Boolean Run(PublishEventJob job)
        {
            Context.System.EventStream.Publish(job.DomainEvent);

            return true;
        }
    }
}
