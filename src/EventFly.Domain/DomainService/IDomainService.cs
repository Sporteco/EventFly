using System.Threading.Tasks;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;

namespace EventFly.DomainService
{
    public interface IDomainService
    {

    }
    public interface IDomainService<TServiceId> : IDomainService
        where TServiceId : IIdentity
    {
        Task<ExecutionResult> PublishCommandAsync<TCommandIdentity, TExecutionResult>(ICommand<TCommandIdentity, TExecutionResult> command)
            where TCommandIdentity : IIdentity where TExecutionResult : IExecutionResult;

    }
}