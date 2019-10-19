using Akka.Actor;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using EventFly.Definitions;
using EventFly.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventFly.Commands
{
    public sealed class CommandToAggregateManagerBus : ICommandBus
    {
        private readonly IDefinitionToManagerRegistry _definitionToManagerRegistry;

        public CommandToAggregateManagerBus(IDefinitionToManagerRegistry definitionToManagerRegistry)
        {
            _definitionToManagerRegistry = definitionToManagerRegistry;
        }

        public Task<TExecutionResult> Publish<TExecutionResult, TIdentity>(ICommand<TIdentity, TExecutionResult> command)
            where TExecutionResult : IExecutionResult
            where TIdentity : IIdentity
        {
            return GetAggregateManager(typeof(TIdentity)).Ask<TExecutionResult>(command, new TimeSpan?());
        }

        public Task<IExecutionResult> Publish(ICommand command)
        {
            return GetAggregateManager(command.GetAggregateId().GetType()).Ask<IExecutionResult>(command, new TimeSpan?());
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
    public interface ICommandBus
    {
        Task<TExecutionResult> Publish<TExecutionResult, TIdentity>(
          ICommand<TIdentity, TExecutionResult> command)
          where TExecutionResult : IExecutionResult
          where TIdentity : IIdentity;

        Task<IExecutionResult> Publish(ICommand command);
    }
}
