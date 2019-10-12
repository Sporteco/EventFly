using Akka.Actor;
using Akkatecture.Commands;
using Akkatecture.Commands.ExecutionResults;
using Akkatecture.Core;
using Akkatecture.Definitions;
using Akkatecture.Jobs;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Akkatecture
{
  public static class SystemHostExtensions
  {
    private static readonly ConcurrentDictionary<int, IApplicationDefinition> _hostmap = new ConcurrentDictionary<int, IApplicationDefinition>();

    private static IApplicationDefinition GetInstance(ActorSystem system)
    {
      return _hostmap.GetOrAdd(system.GetHashCode(), new ApplicationDefinition(system));
    }

    public static IApplicationDefinition GetApplicationDefinition(
      this ActorSystem system)
    {
      return GetInstance(system);
    }

    public static IApplicationDefinition RegisterDomain<TDomainDefinition>(
      this ActorSystem system)
      where TDomainDefinition : IDomainDefinition
    {
      return GetInstance(system).RegisterDomain<TDomainDefinition>();
    }

    public static Task<TExecutionResult> PublishCommandAsync<TIdentity, TExecutionResult>(
      this ActorSystem system,
      ICommand<TIdentity, TExecutionResult> command)
      where TIdentity : IIdentity
      where TExecutionResult : IExecutionResult
    {
      return GetInstance(system).PublishAsync(command);
    }

    public static Task<TResult> ExecuteQueryAsync<TResult>(
      this ActorSystem system,
      Queries.IQuery<TResult> query)
    {
      return GetInstance(system).QueryAsync(query);
    }

    public static IActorRef GetAggregateManager(
      this ActorSystem system,
      Type aggregateType)
    {
      return GetInstance(system).GetAggregateManager(aggregateType);
    }

    public static IActorRef GetSagaManager(this ActorSystem system, Type sagaType)
    {
      return GetInstance(system).GetSagaManager(sagaType);
    }

    public static IEventDefinitions GetEventDefinitions(this ActorSystem system)
    {
      return GetInstance(system).Events;
    }

    public static IJobDefinitions GetJobDefinitions(this ActorSystem system)
    {
      return GetInstance(system).Jobs;
    }

    public static ISnapshotDefinitions GetSnapshotDefinitions(
      this ActorSystem system)
    {
      return GetInstance(system).Snapshots;
    }

    public static IActorRef GetQueryManager(this ActorSystem system, Type queryType)
    {
      return GetInstance(system).GetQueryManager(queryType);
    }

    public static IActorRef GetReadModelManager(
      this ActorSystem system,
      Type readModelType)
    {
      return GetInstance(system).GetReadModelManager(readModelType);
    }
  }
}
