using EventFly.Queries;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventFly.GraphQL
{

    public interface IGraphQueryHandler
    {
        FieldType GetFieldType(Boolean isInput);
        IGraphType GetQueryItemType(Type modelType, Boolean isInput);
        Task<Object> ExecuteQuery(Dictionary<String, Object> arguments);
    }

    public interface IGraphQueryHandler<in TQuery, TResult> : IGraphQueryHandler where TQuery : IQuery<TResult>
    {
    }

}
