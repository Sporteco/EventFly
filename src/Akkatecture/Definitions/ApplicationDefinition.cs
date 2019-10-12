// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.ApplicationDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using Akka.Actor;
using Akkatecture.Commands;
using Akkatecture.Commands.ExecutionResults;
using Akkatecture.Core;
using Akkatecture.Exceptions;
using Akkatecture.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Akkatecture.Definitions
{
  public class ApplicationDefinition : IApplicationDefinition
  {
    private readonly List<IDomainDefinition> _domains = new List<IDomainDefinition>();
    private readonly EventAggregatedDefinitions _events = new EventAggregatedDefinitions();
    private readonly JobAggregatedDefinitions _jobs = new JobAggregatedDefinitions();
    private readonly SnapshotAggregatedDefinitions _snapshots = new SnapshotAggregatedDefinitions();
    private readonly CommandAggregatedDefinitions _commands = new CommandAggregatedDefinitions();
    private readonly ActorSystem _system;

    public IReadOnlyCollection<IDomainDefinition> Domains
    {
      get
      {
        return (IReadOnlyCollection<IDomainDefinition>) this._domains;
      }
    }

    public IReadOnlyCollection<IQueryDefinition> Queries
    {
      get
      {
        return (IReadOnlyCollection<IQueryDefinition>) this._domains.SelectMany<IDomainDefinition, IQueryDefinition>((Func<IDomainDefinition, IEnumerable<IQueryDefinition>>) (i => i.Queries.Select<IQueryDefinition, IQueryDefinition>((Func<IQueryDefinition, IQueryDefinition>) (q => q)))).ToList<IQueryDefinition>();
      }
    }

    public IReadOnlyCollection<IAggregateDefinition> Aggregates
    {
      get
      {
        return (IReadOnlyCollection<IAggregateDefinition>) this._domains.SelectMany<IDomainDefinition, IAggregateDefinition>((Func<IDomainDefinition, IEnumerable<IAggregateDefinition>>) (i => i.Aggregates.Select<IAggregateDefinition, IAggregateDefinition>((Func<IAggregateDefinition, IAggregateDefinition>) (q => q)))).ToList<IAggregateDefinition>();
      }
    }

    public IReadOnlyCollection<IReadModelDefinition> ReadModels
    {
      get
      {
        return (IReadOnlyCollection<IReadModelDefinition>) this._domains.SelectMany<IDomainDefinition, IReadModelDefinition>((Func<IDomainDefinition, IEnumerable<IReadModelDefinition>>) (i => i.ReadModels.Select<IReadModelDefinition, IReadModelDefinition>((Func<IReadModelDefinition, IReadModelDefinition>) (q => q)))).ToList<IReadModelDefinition>();
      }
    }

    public IReadOnlyCollection<ISagaDefinition> Sagas
    {
      get
      {
        return (IReadOnlyCollection<ISagaDefinition>) this._domains.SelectMany<IDomainDefinition, ISagaDefinition>((Func<IDomainDefinition, IEnumerable<ISagaDefinition>>) (i => i.Sagas.Select<ISagaDefinition, ISagaDefinition>((Func<ISagaDefinition, ISagaDefinition>) (q => q)))).ToList<ISagaDefinition>();
      }
    }

    public IEventDefinitions Events
    {
      get
      {
        return (IEventDefinitions) this._events;
      }
    }

    public IJobDefinitions Jobs
    {
      get
      {
        return (IJobDefinitions) this._jobs;
      }
    }

    public ISnapshotDefinitions Snapshots
    {
      get
      {
        return (ISnapshotDefinitions) this._snapshots;
      }
    }

    public ICommandDefinitions Commands
    {
      get
      {
        return (ICommandDefinitions) this._commands;
      }
    }

    public IApplicationDefinition RegisterDomain<TDomainDefinition>() where TDomainDefinition : IDomainDefinition
    {
      IDomainDefinition instance = (IDomainDefinition) Activator.CreateInstance(typeof (TDomainDefinition), (object) this._system);
      this._domains.Add(instance);
      this._events.AddDefinitions(instance.Events);
      this._jobs.AddDefinitions(instance.Jobs);
      this._snapshots.AddDefinitions(instance.Snapshots);
      this._commands.AddDefinitions(instance.Commands);
      return (IApplicationDefinition) this;
    }

    public Task<TExecutionResult> PublishAsync<TExecutionResult, TIdentity>(
      ICommand<TIdentity, TExecutionResult> command)
      where TExecutionResult : IExecutionResult
      where TIdentity : IIdentity
    {
      return Futures.Ask<TExecutionResult>((ICanTell) this.GetAggregateManager(typeof (TIdentity)), (object) command, new TimeSpan?());
    }

    public Task<IExecutionResult> PublishAsync(ICommand command)
    {
      return (Task<IExecutionResult>) Futures.Ask<IExecutionResult>((ICanTell) this.GetAggregateManager(command.GetAggregateId().GetType()), (object) command, new TimeSpan?());
    }

    public Task<TResult> QueryAsync<TResult>(Akkatecture.Queries.IQuery<TResult> query)
    {
      return Futures.Ask<TResult>((ICanTell) this.GetQueryManager(query.GetType()), (object) query, new TimeSpan?());
    }

    public IActorRef GetAggregateManager(Type type)
    {
      IActorRef manager = this.Aggregates.FirstOrDefault<IAggregateDefinition>((Func<IAggregateDefinition, bool>) (i =>
      {
        if (!(i.Type == type))
          return i.IdentityType == type;
        return true;
      }))?.Manager;
      if (manager == null)
        throw new InvalidOperationException("Aggregate " + type.PrettyPrint() + " not registered");
      return manager;
    }

    public IActorRef GetQueryManager(Type queryType)
    {
      IActorRef manager = this.Queries.FirstOrDefault<IQueryDefinition>((Func<IQueryDefinition, bool>) (i => i.Type == queryType))?.Manager;
      if (manager == null)
        throw new InvalidOperationException("Query " + queryType.PrettyPrint() + " not registered");
      return manager;
    }

    public IActorRef GetSagaManager(Type type)
    {
      IActorRef manager = this.Sagas.FirstOrDefault<ISagaDefinition>((Func<ISagaDefinition, bool>) (i =>
      {
        if (!(i.Type == type))
          return i.IdentityType == type;
        return true;
      }))?.Manager;
      if (manager == null)
        throw new InvalidOperationException("Saga " + type.PrettyPrint() + " not registered");
      return manager;
    }

    public IActorRef GetReadModelManager(Type type)
    {
      IActorRef manager = this.ReadModels.FirstOrDefault<IReadModelDefinition>((Func<IReadModelDefinition, bool>) (i => i.Type == type))?.Manager;
      if (manager == null)
        throw new InvalidOperationException("Saga " + type.PrettyPrint() + " not registered");
      return manager;
    }

    public ApplicationDefinition(ActorSystem system)
    {
      this._system = system;
    }
  }
}
