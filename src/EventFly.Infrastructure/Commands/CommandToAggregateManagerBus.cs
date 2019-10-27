using Akka.Actor;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using EventFly.Definitions;
using EventFly.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using EventFly.Extensions;

namespace EventFly.Commands
{
    public sealed class CommandToAggregateManagerBus : ICommandBus
    {
        private readonly IDefinitionToManagerRegistry _definitionToManagerRegistry;
        private readonly IServiceProvider _serviceProvider;

        public CommandToAggregateManagerBus(IDefinitionToManagerRegistry definitionToManagerRegistry, IServiceProvider serviceProvider)
        {
            _definitionToManagerRegistry = definitionToManagerRegistry;
            _serviceProvider = serviceProvider;
        }

        public async Task<ExecutionResult> Publish<TExecutionResult, TIdentity>(ICommand<TIdentity, TExecutionResult> command)
            where TExecutionResult : IExecutionResult
            where TIdentity : IIdentity
        {
            var result = CommandValidationHelper.ValidateCommand(command, _serviceProvider);
            if (!result.IsValid) return new FailedValidationExecutionResult(result);

            var commandResult = await GetAggregateManager(typeof(TIdentity)).Ask<ExecutionResult>(command, new TimeSpan?());
            return commandResult;
        }
        


        public async Task<IExecutionResult> Publish(ICommand command)
        {
            var result = CommandValidationHelper.ValidateCommand(command, _serviceProvider);
            if (!result.IsValid) return new FailedValidationExecutionResult(result);

            return await GetAggregateManager(command.GetAggregateId().GetType()).Ask<IExecutionResult>(command, new TimeSpan?());
        }

        private IActorRef GetAggregateManager(Type type)
        {
            var manager = _definitionToManagerRegistry.DefinitionToAggregateManager.FirstOrDefault(i =>
            {
                var (k, _) = (i.Key, i.Value);
                if (!(k.AggregateType == type))
                    return k.IdentityType == type;
                return true;
            }).Value;
            if (manager == null)
                throw new InvalidOperationException("Aggregate " + type.PrettyPrint() + " not registered");
            return manager;
        }

    }
}
