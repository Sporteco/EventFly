using EventFly.Jobs;

namespace EventFly.Schedulers.Commands
{
    public sealed class PublishCommandJobScheduler : JobScheduler<PublishCommandJobScheduler, PublishCommandJob, PublishCommandJobId> { }
}
