using Akka.Actor;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using EventFly.Definitions;
using EventFly.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventFly.Commands
{
    public class CommandToAggregateManagerBus : ICommandBus
    {
        public CommandToAggregateManagerBus(IDefinitionToManagerRegistry definitionToManagerRegistry, IServiceProvider serviceProvider)
        {
            _definitionToManagerRegistry = definitionToManagerRegistry;
            _serviceProvider = serviceProvider;
        }

        public Task<IExecutionResult> Publish<TIdentity>(ICommand<TIdentity> command)
            where TIdentity : IIdentity
            => PublishInternal(command);

        public Task<IExecutionResult> Publish(ICommand command) => PublishInternal(command);

        protected readonly IServiceProvider _serviceProvider;

        protected virtual Task<IExecutionResult> PublishInternal(ICommand command)
        {
            try
            {
                var validators = _serviceProvider.GetServices<ICommandValidator>();
                foreach (var validator in validators.OrderBy(i => i.Priority))
                {
                    var result = validator.Validate(command);
                    if (!result.IsValid) return Task.FromResult((IExecutionResult)new FailedValidationExecutionResult(result));
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Task.FromResult((IExecutionResult)new UnauthorizedAccessResult());
            }

            var manager = GetAggregateManager(command.GetAggregateId().GetType());
            var executionResult = manager.Ask<IExecutionResult>(command, new TimeSpan?());
            return executionResult;
        }

        protected IActorRef GetAggregateManager(Type type)
        {
            var manager = _definitionToManagerRegistry.DefinitionToAggregateManager.FirstOrDefault(i =>
            {
                var (k, _) = (i.Key, i.Value);
                if (!(k.AggregateType == type))
                    return k.IdentityType == type;
                return true;
            }).Value;
            if (manager == null) throw new InvalidOperationException("Aggregate " + type.PrettyPrint() + " not registered");
            return manager;
        }

        private readonly IDefinitionToManagerRegistry _definitionToManagerRegistry;
    }
}