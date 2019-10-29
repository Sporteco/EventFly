using System;
using System.Threading.Tasks;
using Akka.Actor;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.Definitions;

namespace EventFly.TestFixture.Aggregates
{
    public sealed class TestCommandBus : CommandToAggregateManagerBus
    {
        private IActorRef _senRef;
        public void SetSender(IActorRef senRef)
        {
            _senRef = senRef;
        }

        //var result = CommandValidationHelper.ValidateCommand(command, _serviceProvider);

        protected override Task<IExecutionResult> DoPublish(ICommand command)
        {
            if (_senRef != null)
            {
                GetAggregateManager(command.GetAggregateId().GetType()).Tell(command, _senRef);

                return Task.FromResult(ExecutionResult.Success());
            }

            return base.DoPublish(command);
        }

        public TestCommandBus(IDefinitionToManagerRegistry definitionToManagerRegistry, IServiceProvider serviceProvider) : 
            base(definitionToManagerRegistry, serviceProvider){}
    }
}
