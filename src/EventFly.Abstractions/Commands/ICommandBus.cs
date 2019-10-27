using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using System.Threading.Tasks;

namespace EventFly.Commands
{
    public interface ICommandBus
    {
        Task<ExecutionResult> Publish<TExecutionResult, TIdentity>(
          ICommand<TIdentity, TExecutionResult> command)
          where TExecutionResult : IExecutionResult
          where TIdentity : IIdentity;

        Task<IExecutionResult> Publish(ICommand command);
    }
}
