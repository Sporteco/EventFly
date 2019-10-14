using System.Collections.Generic;
using GraphQL.Types;

namespace Akkatecture.Web.GraphQL
{
    internal sealed class GraphQueryInternal: ObjectGraphType<object>
    {
        public GraphQueryInternal(IEnumerable<IGraphQueryHandler> queries)
        {
            foreach (var query in queries)
            {
                AddField(query.GetFieldType(false));
            }
        }
    }
}