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

namespace EventFly.Web.GraphQL
{
    public sealed class GraphDomain 
    {
        private readonly IDomainDefinition _domainDefinition;
        private readonly IServiceProvider _provider;
        public Dictionary<string,Type> _handlers = new Dictionary<string, Type>();

        public GraphDomain(IDomainDefinition domainDefinition, IServiceProvider provider)
        {
            _domainDefinition = domainDefinition;
            _provider = provider;
            foreach (var query in _domainDefinition.Queries)
            {
                _handlers.Add(query.Name,typeof(IGraphQueryHandler<,>).MakeGenericType(query.Type,query.QueryResultType));
            }
        }

        public FieldType GetFieldType()
        {
            return  new FieldType
            {
                ResolvedType = new ObjectGraphTypeFromDomain(_domainDefinition, _provider),
                Name = !_domainDefinition.Name.EndsWith("Domain") ? _domainDefinition.Name : _domainDefinition.Name.Substring(0, _domainDefinition.Name.Length - "Domain".Length),
                Description = _domainDefinition.GetType().GetCustomAttribute<DescriptionAttribute>()?.Description,
                Arguments = new QueryArguments(),
                Resolver = new FuncFieldResolver<object>(Execute),
            };
        }


        private Task<object> Execute(ResolveFieldContext context)
        {
            foreach (var field in context.Operation.SelectionSet.Children.Cast<Field>())
            {
                foreach (var child in field.SelectionSet.Children.Cast<Field>())
                {
                    var queryName = child.Name;
                    
                    var handler = GetQueryHandler(queryName);
                    var f = handler.GetFieldType(true);
                    
                    var args = ExecutionHelper.GetArgumentValues(context.Schema,f.Arguments, child.Arguments, new Variables());
                    return handler.ExecuteQuery(args);
                }
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