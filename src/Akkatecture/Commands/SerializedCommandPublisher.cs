// Decompiled with JetBrains decompiler
// Type: Akkatecture.Commands.SerializedCommandPublisher
// Assembly: Akkatecture, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.dll

using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Commands.ExecutionResults;
using Akkatecture.Definitions;
using Newtonsoft.Json;

namespace Akkatecture.Commands
{
  public class SerializedCommandPublisher : ISerializedCommandPublisher
  {
    private readonly IApplicationDefinition _applicationDefinition;

    public SerializedCommandPublisher(ActorSystem system)
    {
      _applicationDefinition = system.GetApplicationDefinition();
    }

    public async Task<IExecutionResult> PublishSerilizedCommandAsync(
      string name,
      int version,
      string json,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      if (version <= 0)
        throw new ArgumentOutOfRangeException(nameof (version));
      if (string.IsNullOrEmpty(json))
        throw new ArgumentNullException(nameof (json));
      CommandDefinition commandDefinition;
      if (!_applicationDefinition.Commands.TryGetDefinition(name, version, out commandDefinition))
        throw new ArgumentException($"No command definition found for command '{name}' v{version}");
      ICommand command;
      try
      {
        command = (ICommand) JsonConvert.DeserializeObject(json, commandDefinition.Type);
      }
      catch (Exception ex)
      {
        throw new ArgumentException($"Failed to deserialize command '{name}' v{version}: {ex.Message}", ex);
      }
      var executionResult = await _applicationDefinition.PublishAsync(command);
      return executionResult;
    }
  }
}
