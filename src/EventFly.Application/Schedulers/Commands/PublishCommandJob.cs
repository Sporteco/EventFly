using EventFly.Commands;
using EventFly.Jobs;

namespace EventFly.Schedulers.Commands
{
    [JobName("publish-command-job")]
    public sealed class PublishCommandJob : Job<PublishCommandJobId>
    {
        public PublishCommandJob(PublishCommandJobId jobId, ICommand command)
            : base(jobId)
        {
            Command = command;
        }

        public ICommand Command { get; }
    }
}
