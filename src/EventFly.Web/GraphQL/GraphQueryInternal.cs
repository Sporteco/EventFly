using System;
using System.Linq;
using EventFly.Definitions;
using GraphQL.Types;

namespace EventFly.Web.GraphQL
{
    internal sealed class GraphQueryInternal: ObjectGraphType<object>
    {
        public GraphQueryInternal(IApplicationDefinition app, IServiceProvider provider)
        {
            foreach (var domain in app.Contexts.Where(i=>i.Queries.Any()))
            {
                var graphDomain = new GraphDomain(domain,provider);
                AddField(graphDomain.GetFieldType());
            }
        }
    }
}