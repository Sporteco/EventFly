using EventFly.Commands;
using EventFly.Schedulers.Commands;
using System;
using System.Threading.Tasks;

namespace EventFly.Schedulers
{
    public interface ICommandsScheduler
    {
        Task<Boolean> Schedule(PublishCommandJobId id, ICommand command, DateTime triggerDate);

        Task<Boolean> Schedule(PublishCommandJobId id, ICommand command, TimeSpan interval, DateTime triggerDate);

        Task<Boolean> Schedule(PublishCommandJobId id, ICommand command, String cronExpression, DateTime triggerDate);

        Task<Boolean> Cancel(PublishCommandJobId jobId);
    }
}
