using Akka.Actor;
using EventFly.Definitions;
using EventFly.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventFly.Queries
{
    public sealed class QueryToQueryHandlerProcessor : IQueryProcessor
    {
        private readonly IDefinitionToManagerRegistry _definitionToManagerRegistry;

        public QueryToQueryHandlerProcessor(IDefinitionToManagerRegistry definitionToManagerRegistry)
        {
            _definitionToManagerRegistry = definitionToManagerRegistry;
        }

        public Task<TResult> Process<TResult>(IQuery<TResult> query)
        {
            return GetQueryManager(query.GetType()).Ask<TResult>(query, new TimeSpan?());
        }

        public Task<Object> Process(IQuery query)
        {
            return GetQueryManager(query.GetType()).Ask(query, new TimeSpan?());
        }

        private IActorRef GetQueryManager(Type queryType)
        {
            var manager = _definitionToManagerRegistry.DefinitionToQueryManager.FirstOrDefault(i =>
            {
                var (k, _) = (i.Key, i.Value);

                return k.QueryType == queryType;
            }).Value;
            if (manager == null)
                throw new InvalidOperationException("Query " + queryType.PrettyPrint() + " not registered");
            return manager;
        }
    }
}
