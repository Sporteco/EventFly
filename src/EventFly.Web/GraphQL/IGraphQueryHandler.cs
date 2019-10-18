using System;
using EventFly.Queries;
using GraphQL.Types;

namespace EventFly.Web.GraphQL
{

    public interface IGraphQueryHandler
    {
        FieldType GetFieldType(bool isInput);
        IGraphType GetQueryItemType(Type modelType, bool isInput);
    }
    
    public interface IGraphQueryHandler<in TQuery, TResult> : IGraphQueryHandler where TQuery : IQuery<TResult>
    {
    }

}
