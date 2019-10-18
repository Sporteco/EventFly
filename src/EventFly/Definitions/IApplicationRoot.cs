// Decompiled with JetBrains decompiler
// Type: EventFly.Definitions.IApplicationDefinition
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using System.Threading.Tasks;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using EventFly.Queries;

namespace EventFly.Definitions
{
    public interface IApplicationRoot
    {

        Task<TExecutionResult> PublishAsync<TExecutionResult, TIdentity>(
          ICommand<TIdentity, TExecutionResult> command)
          where TExecutionResult : IExecutionResult
          where TIdentity : IIdentity;

        Task<IExecutionResult> PublishAsync(ICommand command);

        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
        Task<object> QueryAsync(IQuery query);
    }
}
