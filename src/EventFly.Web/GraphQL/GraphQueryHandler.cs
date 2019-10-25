using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using EventFly.Queries;
using GraphQL.Resolvers;
using GraphQL.Types;
using Newtonsoft.Json;

namespace EventFly.GraphQL
{
    public sealed class GraphQueryHandler<TQuery, TResult> : IGraphQueryHandler<TQuery,TResult> where TQuery : IQuery<TResult> 
    {
        private readonly IQueryProcessor _queryProcessor;

        public GraphQueryHandler(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        public async Task<object> ExecuteAsync(object query)
        {
            return await ReadAsync((TQuery)query);
        }

        private FieldType _fieldType;
        public FieldType GetFieldType(bool isInput)
        {
            if (_fieldType != null) return _fieldType;

            var name = typeof(TQuery).Name;
            name = !name.EndsWith("Query") ? name : name.Substring(0, name.Length - "Query".Length);
            _fieldType = new FieldType
            {
                ResolvedType = QueryParametersHelper.GetQueryItemType(this, typeof(TResult),false),
                Name = name,
                Description = typeof(TQuery).GetCustomAttribute<DescriptionAttribute>()?.Description,
                Arguments = QueryParametersHelper.GetArguments(typeof(TQuery), this, true),
                Resolver = new FuncFieldResolver<TResult>(context => ExecuteQuery(context).GetAwaiter().GetResult()),
            };
            return _fieldType;
        }

        public IGraphType GetQueryItemType(Type modelType, bool isInput)
        {
            return QueryParametersHelper.GetQueryItemType(this, modelType, false);
        }

        private Task<TResult> ReadAsync(TQuery query)
        {
            return _queryProcessor.Process(query);
        }

        public async Task<object> ExecuteQuery(Dictionary<string, object> arguments)
        {
            return await ReadAsync(ParseModel<TQuery>(arguments));
        }

        private Task<TResult> ExecuteQuery(ResolveFieldContext context) => ReadAsync(ParseModel<TQuery>(context.Arguments));

        private T ParseModel<T>(Dictionary<string, object> arguments) where T : IQuery<TResult>
            => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(arguments));
       
        public static Func<object,object> ConvertFunc<TParent, TRes>(Func<TParent, TRes> func) => arg => func((TParent)arg);
    }
}