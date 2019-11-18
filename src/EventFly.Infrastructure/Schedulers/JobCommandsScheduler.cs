using EventFly.Commands;
using EventFly.Jobs;
using EventFly.Schedulers.Commands;
using System;
using System.Threading.Tasks;

namespace EventFly.Schedulers
{
    internal sealed class JobCommandsScheduler : ICommandsScheduler
    {
        private readonly IScheduler _scheduler;

        public JobCommandsScheduler(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public Task<Boolean> Schedule(PublishCommandJobId id, ICommand command, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishCommandJob, PublishCommandJobId>(new PublishCommandJob(id, command), triggerDate);
        }

        public Task<Boolean> Schedule(PublishCommandJobId id, ICommand command, TimeSpan interval, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishCommandJob, PublishCommandJobId>(new PublishCommandJob(id, command), interval, triggerDate);
        }

        public Task<Boolean> Schedule(PublishCommandJobId id, ICommand command, String cronExpression, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishCommandJob, PublishCommandJobId>(new PublishCommandJob(id, command), cronExpression, triggerDate);
        }

        public Task<Boolean> Cancel(PublishCommandJobId jobId)
        {
            return _scheduler.Cancel<PublishCommandJob, PublishCommandJobId>(jobId);
        }
    }
}
