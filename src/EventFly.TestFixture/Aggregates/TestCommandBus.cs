using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using EventFly.Definitions;
using EventFly.Exceptions;
using EventFly.Extensions;

namespace EventFly.TestFixture.Aggregates
{
    public interface ITestCommandBus
    {
        void Publish(ICommand command, IActorRef sender);
    }
    public sealed class TestCommandBus : ICommandBus, ITestCommandBus
    {
        private readonly IDefinitionToManagerRegistry _definitionToManagerRegistry;
        private readonly IServiceProvider _serviceProvider;

        public TestCommandBus(IDefinitionToManagerRegistry definitionToManagerRegistry, IServiceProvider serviceProvider)
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

        public void Publish(ICommand command, IActorRef sender)
        {
            var result = CommandValidationHelper.ValidateCommand(command, _serviceProvider);
            if (!result.IsValid)
            {
                sender.Tell(new FailedValidationExecutionResult(result));
                return;
            }

            GetAggregateManager(command.GetAggregateId().GetType()).Tell(command, sender);
        }
    }
}
