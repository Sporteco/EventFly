using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using System.Threading.Tasks;

namespace EventFly.Commands
{
    public interface ICommandBus
    {
        Task<IExecutionResult> Publish<TIdentity>(
          ICommand<TIdentity> command)
          where TIdentity : IIdentity;

        Task<IExecutionResult> Publish(ICommand command);
    }
}
