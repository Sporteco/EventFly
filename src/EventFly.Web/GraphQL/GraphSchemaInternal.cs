using System;
using GraphQL.Types;

namespace EventFly.Web.GraphQL
{
    internal class GraphSchemaInternal : Schema
    {
        public GraphSchemaInternal(IServiceProvider provider) : base(type => (IGraphType) provider.GetService(type))
        {
            Query = (GraphQueryInternal)provider.GetService(typeof(GraphQueryInternal));
        }
    }
}