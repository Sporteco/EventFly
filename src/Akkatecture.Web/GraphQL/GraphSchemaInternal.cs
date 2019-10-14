using GraphQL;
using GraphQL.Types;

namespace Akkatecture.Web.GraphQL
{
    internal class GraphSchemaInternal : Schema
    {
        public GraphSchemaInternal(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<GraphQueryInternal>();
        }
    }
}