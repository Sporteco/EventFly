using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EventFly.Definitions;
using GraphQL.Execution;
using GraphQL.Language.AST;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace EventFly.GraphQL
{
    public sealed class GraphDomain 
    {
        private readonly IContextDefinition _domainDefinition;
        private readonly IServiceProvider _provider;
        private readonly Dictionary<string,Type> _handlers = new Dictionary<string, Type>();

        public GraphDomain(IContextDefinition domainDefinition, IServiceProvider provider)
        {
            _domainDefinition = domainDefinition;
            _provider = provider;
            foreach (var query in _domainDefinition.Queries)
            {
                var name = query.Type.Name;
                name = !name.EndsWith("Query") ? name : name.Substring(0, name.Length - "Query".Length);
                _handlers.Add(name,typeof(IGraphQueryHandler<,>).MakeGenericType(query.Type,query.QueryResultType));
            }
        }

        public FieldType GetFieldType()
        {
            return  new FieldType
            {
                ResolvedType = new ObjectGraphTypeFromDomain(_domainDefinition, _provider),
                Name = GetDomainName(_domainDefinition.Name),
                Description = _domainDefinition.GetType().GetCustomAttribute<DescriptionAttribute>()?.Description,
                Arguments = new QueryArguments(),
                Resolver = new FuncFieldResolver<object>(Execute),
            };
        }

        private string GetDomainName(string name)
        {
            return !name.EndsWith("Context") ? name : name.Substring(0, name.Length - "Context".Length);
        }


        private Task<object> Execute(ResolveFieldContext context)
        {
            foreach (var field in context.FieldAst.SelectionSet.Children.Cast<Field>())
            {
                var queryName = field.Name;

                var handler = GetQueryHandler(queryName);
                var f = handler.GetFieldType(true);
                
                var args = ExecutionHelper.GetArgumentValues(context.Schema,f.Arguments, field.Arguments, new Variables());
                return handler.ExecuteQuery(args);
            }

            return null;
        }


        private IGraphQueryHandler GetQueryHandler(string queryName)
        {
            return (IGraphQueryHandler)_provider.GetService(
                _handlers.FirstOrDefault(i=>i.Key.Equals(queryName,StringComparison.InvariantCultureIgnoreCase)).Value);
        }
    }
}