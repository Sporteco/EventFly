// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.IApplicationDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Commands;
using Akkatecture.Commands.ExecutionResults;
using Akkatecture.Core;
using Akkatecture.Jobs;
using Akkatecture.Queries;

namespace Akkatecture.Definitions
{
  public interface IApplicationDefinition
  {
    IReadOnlyCollection<IDomainDefinition> Domains { get; }

    IReadOnlyCollection<IQueryDefinition> Queries { get; }

    IReadOnlyCollection<IAggregateDefinition> Aggregates { get; }

    IReadOnlyCollection<IReadModelDefinition> ReadModels { get; }

    IReadOnlyCollection<ISagaDefinition> Sagas { get; }

    ICommandDefinitions Commands { get; }

    IEventDefinitions Events { get; }

    IJobDefinitions Jobs { get; }

    ISnapshotDefinitions Snapshots { get; }

    IApplicationDefinition RegisterDomain<TDomainDefinition>() where TDomainDefinition : IDomainDefinition;

    Task<TExecutionResult> PublishAsync<TExecutionResult, TIdentity>(
      ICommand<TIdentity, TExecutionResult> command)
      where TExecutionResult : IExecutionResult
      where TIdentity : IIdentity;

    Task<IExecutionResult> PublishAsync(ICommand command);

    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);

    IActorRef GetAggregateManager(Type aggregateType);

    IActorRef GetQueryManager(Type queryType);

    IActorRef GetSagaManager(Type sagaType);

    IActorRef GetReadModelManager(Type readModelType);
  }
}
