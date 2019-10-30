using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using System.Threading.Tasks;

namespace EventFly.Domain
{
    public interface IDomainService
    {
        Task<IExecutionResult> PublishCommandAsync<TCommandIdentity>(ICommand<TCommandIdentity> command)
            where TCommandIdentity : IIdentity;
    }
}