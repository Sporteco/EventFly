using System;
using Akkatecture.Queries;
using GraphQL.Types;

namespace Akkatecture.Web.GraphQL
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
