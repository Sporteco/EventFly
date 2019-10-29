using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.Definitions;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.TestFixture.Aggregates
{
    public sealed class TestCommandBus : CommandToAggregateManagerBus
    {
        private IActorRef _senRef;
        public void SetSender(IActorRef senRef)
        {
            _senRef = senRef;
        }

        protected override Task<IExecutionResult> PublishInternal(ICommand command)
        {
            try
            {
                var validators = _serviceProvider.GetServices<ICommandValidator>();
                foreach (var validator in validators.OrderBy(i => i.Priority))
                {
                    var result = validator.Validate(command);
                    if (!result.IsValid) return Task.FromResult((IExecutionResult) new FailedValidationExecutionResult(result));
                }
            }
            catch (UnauthorizedAccessException)
            {
                _senRef?.Tell(new UnauthorizedAccessResult());

                return Task.FromResult((IExecutionResult) new UnauthorizedAccessResult());
            }

            if (_senRef != null)
            {
                GetAggregateManager(command.GetAggregateId().GetType()).Tell(command, _senRef);

                return Task.FromResult(ExecutionResult.Success());
            }


            return GetAggregateManager(command.GetAggregateId().GetType()).Ask<IExecutionResult>(command, new TimeSpan?());
        }



        public TestCommandBus(IDefinitionToManagerRegistry definitionToManagerRegistry, IServiceProvider serviceProvider) : 
            base(definitionToManagerRegistry, serviceProvider){}
    }
}
