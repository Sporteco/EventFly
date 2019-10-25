using EventFly.Commands;
using EventFly.Schedulers.Commands;
using System;
using System.Threading.Tasks;

namespace EventFly.Schedulers
{
    public interface ICommandsScheduler
    {
        Task<bool> Schedule(PublishCommandJobId id, ICommand command, DateTime triggerDate);

        Task<bool> Schedule(PublishCommandJobId id, ICommand command, TimeSpan interval, DateTime triggerDate);

        Task<bool> Schedule(PublishCommandJobId id, ICommand command, string cronExpression, DateTime triggerDate);

        Task<bool> Cancel(PublishCommandJobId jobId);
    }
}
