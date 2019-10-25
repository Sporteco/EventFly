using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventFly.Queries;
using GraphQL.Types;

namespace EventFly.GraphQL
{

    public interface IGraphQueryHandler
    {
        FieldType GetFieldType(bool isInput);
        IGraphType GetQueryItemType(Type modelType, bool isInput);
        Task<object> ExecuteQuery(Dictionary<string, object> arguments);
    }
    
    public interface IGraphQueryHandler<in TQuery, TResult> : IGraphQueryHandler where TQuery : IQuery<TResult>
    {
    }

}
