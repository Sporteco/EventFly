using GraphQL;
using GraphQL.Types;

namespace EventFly.Web.GraphQL
{
    internal class GraphSchemaInternal : Schema
    {
        public GraphSchemaInternal(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<GraphQueryInternal>();
        }
    }
}