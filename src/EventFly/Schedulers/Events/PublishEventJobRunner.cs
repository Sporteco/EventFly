using EventFly.Jobs;

namespace EventFly.Schedulers.Events
{
    public sealed class PublishEventJobRunner : JobRunner<PublishEventJob, PublishEventJobId>,
        IRun<PublishEventJob, PublishEventJobId>

    {
        public bool Run(PublishEventJob job)
        {
            Context.System.EventStream.Publish(job.DomainEvent);

            return true;
        }
    }
}
