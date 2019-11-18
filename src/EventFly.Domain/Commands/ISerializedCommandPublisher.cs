// Decompiled with JetBrains decompiler
// Type: EventFly.Commands.ISerializedCommandPublisher
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using EventFly.Commands.ExecutionResults;
using System.Threading;
using System.Threading.Tasks;

namespace EventFly.Commands
{
    public interface ISerializedCommandPublisher
    {
        Task<IExecutionResult> PublishSerilizedCommandAsync(
          System.String name,
          System.Int32 version,
          System.String json,
          CancellationToken cancellationToken);
    }
}
