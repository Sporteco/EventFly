using EventFly.Infrastructure.Jobs;
using EventFly.Schedulers.Commands;

namespace EventFly.Infrastructure.Schedulers.Commands
{
    public sealed class PublishCommandJobScheduler : JobScheduler<PublishCommandJobScheduler, PublishCommandJob, PublishCommandJobId> { }
}
