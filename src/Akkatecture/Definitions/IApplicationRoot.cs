// Decompiled with JetBrains decompiler
// Type: Akkatecture.Definitions.IApplicationDefinition
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using System.Threading.Tasks;
using Akkatecture.Commands;
using Akkatecture.Commands.ExecutionResults;
using Akkatecture.Core;
using Akkatecture.Queries;

namespace Akkatecture.Definitions
{
    public interface IApplicationRoot
    {
        IApplicationDefinition Definitions { get; }

        Task<TExecutionResult> PublishAsync<TExecutionResult, TIdentity>(
          ICommand<TIdentity, TExecutionResult> command)
          where TExecutionResult : IExecutionResult
          where TIdentity : IIdentity;

        Task<IExecutionResult> PublishAsync(ICommand command);

        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
        Task<object> QueryAsync(IQuery query);
    }
}
