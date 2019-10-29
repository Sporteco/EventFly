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
        Task<IExecutionResult> PublishCommandAsync<TCommandIdentity>(ICommand<TCommandIdentity> command)
            where TCommandIdentity : IIdentity;

    }
}