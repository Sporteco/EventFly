using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Definitions;
using Akkatecture.Queries;
using GraphQL.Resolvers;
using GraphQL.Types;
using Newtonsoft.Json;

namespace Akkatecture.Web.GraphQL
{
    public sealed class GraphQueryHandler<TQuery, TResult> : IGraphQueryHandler<TQuery,TResult> where TQuery : IQuery<TResult> 
    {
        private readonly IApplicationDefinition _definitions;

        public GraphQueryHandler(IApplicationDefinition definitions)
        {
            _definitions = definitions;
        }

        public async Task<object> ExecuteAsync(object query)
        {
            return await ReadAsync((TQuery)query);
        }

        public FieldType GetFieldType(bool isInput)
        {
            return  new FieldType
            {
                ResolvedType = GetQueryItemType(typeof(TResult),isInput),
                Name = typeof(TQuery).Name,
                Description = typeof(TQuery).GetCustomAttribute<DescriptionAttribute>()?.Description,
                Arguments = QueryParametersHelper.GetArguments(typeof(TQuery), this),
                Resolver = new FuncFieldResolver<TResult>(context => ExecuteQuery(context).GetAwaiter().GetResult()),
            };
        }

        private Task<TResult> ReadAsync(TQuery query)
        {
            return _definitions.QueryAsync(query);
        }

        private Task<TResult> ExecuteQuery(ResolveFieldContext context) => ReadAsync(ParseModel<TQuery>(context.Arguments));

        private T ParseModel<T>(Dictionary<string, object> arguments) where T : IQuery<TResult>
            => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(arguments));


        public IGraphType GetQueryItemType(Type modelType, bool isInput)
        {
            lock (_declaredTypes)
            {
                if (_declaredTypes.ContainsKey(modelType)) return _declaredTypes[modelType];
                var result = GetGraphTypeEx(modelType, this,isInput);
                _declaredTypes.Add(modelType, result);
                return result;
            }
        }
        private readonly Dictionary<Type,IGraphType> _declaredTypes = new Dictionary<Type, IGraphType>();

        public static Func<object,object> ConvertFunc<TParent, TRes>(Func<TParent, TRes> func) => arg => func((TParent)arg);

        private static IGraphType GetGraphTypeEx(Type propType, IGraphQueryHandler graphQuery, bool isInput)
        {
            if (propType.IsGenericType && propType.GetGenericArguments().Length == 1 && typeof(IEnumerable).IsAssignableFrom(propType))
            {
                var innerType = propType.GetGenericArguments().First();
                if (innerType != null)
                    return new ListGraphType(graphQuery.GetQueryItemType(innerType,isInput));
                return null;
            }

            return isInput ? (IGraphType) new InputObjectGraphTypeFromModel(propType, graphQuery) 
                : new ObjectGraphTypeFromModel(propType, graphQuery);
        }
    }
}