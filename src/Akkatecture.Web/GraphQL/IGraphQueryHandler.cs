using System;
using Akkatecture.Queries;
using GraphQL.Types;

namespace Akkatecture.Web.GraphQL
{

    public interface IGraphQueryHandler
    {
        FieldType GetFieldType();
        IGraphType GetQueryItemType(Type modelType);
    }
    
    public interface IGraphQueryHandler<in TQuery, TResult> : IGraphQueryHandler where TQuery : IQuery<TResult>
    {
    }

}
