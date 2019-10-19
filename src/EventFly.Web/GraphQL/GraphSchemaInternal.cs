using System;
using GraphQL.Types;

namespace EventFly.Web.GraphQL
{
    internal class GraphSchemaInternal : Schema
    {
#pragma warning disable 618
        public GraphSchemaInternal(IServiceProvider provider) : base(type =>
#pragma warning restore 618
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