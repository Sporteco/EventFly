using EventFly.Infrastructure.Jobs;
using EventFly.Schedulers.Events;

namespace EventFly.Infrastructure.Schedulers.Events
{
    public sealed class PublishEventJobScheduler : JobScheduler<PublishEventJobScheduler, PublishEventJob, PublishEventJobId> { }
}
