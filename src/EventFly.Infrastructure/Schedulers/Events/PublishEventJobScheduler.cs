using EventFly.Jobs;

namespace EventFly.Schedulers.Events
{
    public sealed class PublishEventJobScheduler : JobScheduler<PublishEventJobScheduler, PublishEventJob, PublishEventJobId> { }
}
