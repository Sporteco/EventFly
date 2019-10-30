using System;
using System.Threading.Tasks;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;

namespace EventFly.DomainService
{
    public interface IDomainService
    {
        Task<IExecutionResult> PublishCommandAsync<TCommandIdentity>(ICommand<TCommandIdentity> command)
            where TCommandIdentity : IIdentity;
    }

    public interface IDomainServiceHandles<in TIdentity, in TAggregateEvent>
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
        where TIdentity : IIdentity
    {
        Boolean Handle(IDomainEvent<TIdentity, TAggregateEvent> domainEvent);
    }

    public interface IDomainServiceHandlesAsync<in TIdentity, in TAggregateEvent>
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
        where TIdentity : IIdentity
    {
        Task HandleAsync(IDomainEvent<TIdentity, TAggregateEvent> domainEvent);
    }

    public interface IDomainServiceIsStartedBy<in TIdentity, in TAggregateEvent> : IDomainServiceHandles<TIdentity, TAggregateEvent>
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
        where TIdentity : IIdentity
    { }

    public interface IDomainServiceIsStartedByAsync<in TIdentity, in TAggregateEvent> : IDomainServiceHandlesAsync<TIdentity, TAggregateEvent>
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
        where TIdentity : IIdentity
    { }
}