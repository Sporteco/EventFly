using System;
using System.Collections.Concurrent;
using EventFly.DependencyInjection;
using GraphQL;
using GraphQL.Http;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.Web.GraphQL
{
    public static class BuilderExtensions
    {
        private static ConcurrentDictionary<Type,object> _enumCache = new ConcurrentDictionary<Type,object>(); 
        private static object GetRequiredServiceEx(this IServiceProvider provider, Type serviceType)
        {
            if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(EnumerationGraphType<>))
            {
                return _enumCache.GetOrAdd(serviceType, (у) => Activator.CreateInstance(serviceType));
            }
            return provider.GetRequiredService(serviceType);
        }

        public static EventFlyBuilder AddGraphQl(this EventFlyBuilder builder)
        {
            var services = builder.Services;
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddSingleton<GraphQueryInternal>();
            services.AddSingleton<ISchema, GraphSchemaInternal>();
            services.AddGraphQL(_ =>
            {
                _.EnableMetrics = true;
                _.ExposeExceptions = true;
            });

            foreach (var query in builder.ApplicationDefinition.Queries)
            {
                var handlerType = typeof(GraphQueryHandler<,>).MakeGenericType(query.Type, query.QueryResultType);
                var handlerFullType = typeof(IGraphQueryHandler<,>).MakeGenericType(query.Type, query.QueryResultType);
                services.AddSingleton(handlerFullType,handlerType);
                //services.AddSingleton(provider => (IGraphQueryHandler) provider.GetService(handlerFullType));
            }

            return builder;
        }
        public static IApplicationBuilder UseEventFlyGraphQl(this IApplicationBuilder app)
        {
	        
            app.UseGraphQL<ISchema>();

            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
            {
                Path = "/graphql-console"
            });
            return app;
        }
    }
}
