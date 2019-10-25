using EventFly.Commands;
using EventFly.Schedulers.Commands;
using EventFly.Jobs;
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

        public Task<bool> Schedule(PublishCommandJobId id, ICommand command, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishCommandJob, PublishCommandJobId>(new PublishCommandJob(id, command), triggerDate);
        }

        public Task<bool> Schedule(PublishCommandJobId id, ICommand command, TimeSpan interval, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishCommandJob, PublishCommandJobId>(new PublishCommandJob(id, command), interval, triggerDate);
        }

        public Task<bool> Schedule(PublishCommandJobId id, ICommand command, string cronExpression, DateTime triggerDate)
        {
            return _scheduler.Schedule<PublishCommandJob, PublishCommandJobId>(new PublishCommandJob(id, command), cronExpression, triggerDate);
        }

        public Task<bool> Cancel(PublishCommandJobId jobId)
        {
            return _scheduler.Cancel<PublishCommandJob, PublishCommandJobId>(jobId);
        }
    }
}
