// Decompiled with JetBrains decompiler
// Type: EventFly.Commands.SerializedCommandPublisher
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using EventFly.Commands.ExecutionResults;
using EventFly.Definitions;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventFly.Commands
{
    public class SerializedCommandPublisher : ISerializedCommandPublisher
    {
        private readonly IApplicationDefinition _applicationDefinition;
        private readonly ICommandBus _bus;

        public SerializedCommandPublisher(IApplicationDefinition applicationDefinition, ICommandBus bus)
        {
            _applicationDefinition = applicationDefinition;
            _bus = bus;
        }

        public async Task<IExecutionResult> PublishSerilizedCommandAsync(
          String name,
          Int32 version,
          String json,
          CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (version <= 0)
                throw new ArgumentOutOfRangeException(nameof(version));
            if (String.IsNullOrEmpty(json))
                throw new ArgumentNullException(nameof(json));

            if (!_applicationDefinition.Commands.TryGetDefinition(name, version, out CommandDefinition commandDefinition))
                throw new ArgumentException($"No command definition found for command '{name}' v{version}");

            ICommand command;
            try
            {
                command = (ICommand)JsonConvert.DeserializeObject(json, commandDefinition.Type);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to deserialize command '{name}' v{version}: {ex.Message}", ex);
            }
            var executionResult = await _bus.Publish(command);
            return executionResult;
        }
    }
}
