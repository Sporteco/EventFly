using System;
using GraphQL.Types;

namespace EventFly.Web.GraphQL
{
    internal class GraphSchemaInternal : Schema
    {
        public GraphSchemaInternal(IServiceProvider provider) : base(type =>
        {
            var result = (IGraphType) provider.GetService(type);
            if (result == null && type.GetGenericTypeDefinition() == typeof(EnumerationGraphType<>))
            {
                //TODO: Refactoring
                return (IGraphType) Activator.CreateInstance(type);
            }

            return null;
        })
        {
            Query = (GraphQueryInternal)provider.GetService(typeof(GraphQueryInternal));
        }
    }
}