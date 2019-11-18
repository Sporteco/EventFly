using EventFly.Queries;
using GraphQL.Resolvers;
using GraphQL.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace EventFly.GraphQL
{
    public sealed class GraphQueryHandler<TQuery, TResult> : IGraphQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly IQueryProcessor _queryProcessor;

        public GraphQueryHandler(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        public async Task<Object> ExecuteAsync(Object query)
        {
            return await ReadAsync((TQuery)query);
        }

        private FieldType _fieldType;
        public FieldType GetFieldType(Boolean isInput)
        {
            if (_fieldType != null) return _fieldType;

            var name = typeof(TQuery).Name;
            name = !name.EndsWith("Query") ? name : name.Substring(0, name.Length - "Query".Length);
            _fieldType = new FieldType
            {
                ResolvedType = QueryParametersHelper.GetQueryItemType(this, typeof(TResult), false),
                Name = name,
                Description = typeof(TQuery).GetCustomAttribute<DescriptionAttribute>()?.Description,
                Arguments = QueryParametersHelper.GetArguments(typeof(TQuery), this, true),
                Resolver = new FuncFieldResolver<TResult>(context => ExecuteQuery(context).GetAwaiter().GetResult()),
            };
            return _fieldType;
        }

        public IGraphType GetQueryItemType(Type modelType, Boolean isInput)
        {
            return QueryParametersHelper.GetQueryItemType(this, modelType, isInput);
        }

        private Task<TResult> ReadAsync(TQuery query)
        {
            return _queryProcessor.Process(query);
        }

        public async Task<Object> ExecuteQuery(Dictionary<String, Object> arguments)
        {
            return await ReadAsync(ParseModel<TQuery>(arguments));
        }

        private Task<TResult> ExecuteQuery(ResolveFieldContext context) => ReadAsync(ParseModel<TQuery>(context.Arguments));

        private T ParseModel<T>(Dictionary<String, Object> arguments) where T : IQuery<TResult>
            => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(arguments ?? new Dictionary<String, Object>()));

        public static Func<Object, Object> ConvertFunc<TParent, TRes>(Func<TParent, TRes> func) => arg => func((TParent)arg);
    }
}