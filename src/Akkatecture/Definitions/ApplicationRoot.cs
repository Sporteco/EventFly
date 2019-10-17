using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Commands;
using Akkatecture.Commands.ExecutionResults;
using Akkatecture.Core;
using Akkatecture.Exceptions;
using Akkatecture.Queries;

namespace Akkatecture.Definitions
{


    /// <summary>
    /// God object that runs everything
    /// </summary>
    internal sealed class ApplicationRoot : IApplicationRoot
    {
        public ApplicationRoot(IApplicationDefinition definitions)
        {
            Definitions = definitions;
        }

        public IApplicationDefinition Definitions { get; }

        public Task<TExecutionResult> PublishAsync<TExecutionResult, TIdentity>(
          ICommand<TIdentity, TExecutionResult> command)
          where TExecutionResult : IExecutionResult
          where TIdentity : IIdentity
        {
            return GetAggregateManager(typeof(TIdentity)).Ask<TExecutionResult>(command, new TimeSpan?());
        }

        public Task<IExecutionResult> PublishAsync(ICommand command)
        {
            return GetAggregateManager(command.GetAggregateId().GetType()).Ask<IExecutionResult>(command, new TimeSpan?());
        }

        public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            return GetQueryManager(query.GetType()).Ask<TResult>(query, new TimeSpan?());
        }

        public Task<object> QueryAsync(IQuery query)
        {
            return GetQueryManager(query.GetType()).Ask(query, new TimeSpan?());
        }

        internal IActorRef GetAggregateManager(Type type)
        {
            var manager = Definitions.Aggregates.FirstOrDefault(i =>
            {
                if (!(i.Type == type))
                    return i.IdentityType == type;
                return true;
            })?.Manager;
            if (manager == null)
                throw new InvalidOperationException("Aggregate " + type.PrettyPrint() + " not registered");
            return manager;
        }

        internal IActorRef GetQueryManager(Type queryType)
        {
            var manager = Definitions.Queries.FirstOrDefault(i => i.Type == queryType)?.Manager;
            if (manager == null)
                throw new InvalidOperationException("Query " + queryType.PrettyPrint() + " not registered");
            return manager;
        }

        internal IActorRef GetSagaManager(Type type)
        {
            var manager = Definitions.Sagas.FirstOrDefault(i =>
            {
                if (!(i.Type == type))
                    return i.IdentityType == type;
                return true;
            })?.Manager;
            if (manager == null)
                throw new InvalidOperationException("Saga " + type.PrettyPrint() + " not registered");
            return manager;
        }

        internal IActorRef GetReadModelManager(Type type)
        {
            var manager = Definitions.ReadModels.FirstOrDefault(i => i.Type == type)?.Manager;
            if (manager == null)
                throw new InvalidOperationException("Saga " + type.PrettyPrint() + " not registered");
            return manager;
        }
    }
}
