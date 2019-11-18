using EventFly.Definitions;
using EventFly.DependencyInjection;
using GraphQL;
using GraphQL.Http;
using GraphQL.Server;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace EventFly.GraphQL
{
    public sealed class EventFlyGraphQlOptions
    {
        public EventFlyGraphQlOptions(String basePath)
        {
            BasePath = basePath;
        }

        public String BasePath { get; set; }
    }

    public sealed class EventFlyGraphQlConsoleOptions
    {
        public EventFlyGraphQlConsoleOptions(String basePath)
        {
            BasePath = basePath;
        }

        public String BasePath { get; set; }
    }

    public sealed class EventFlyGraphQlBuilder
    {
        public EventFlyGraphQlBuilder(EventFlyBuilder builder)
        {
            Builder = builder;
        }

        public EventFlyBuilder Builder { get; }
        public IServiceCollection Services => Builder.Services;
        public IApplicationDefinition ApplicationDefinition => Builder.ApplicationDefinition;
    }

    public static class BuilderExtensions
    {
        private static ConcurrentDictionary<Type, Object> _enumCache = new ConcurrentDictionary<Type, Object>();
        private static Object GetRequiredServiceEx(this IServiceProvider provider, Type serviceType)
        {
            if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(EnumerationGraphType<>))
            {
                return _enumCache.GetOrAdd(serviceType, (у) => Activator.CreateInstance(serviceType));
            }
            return provider.GetRequiredService(serviceType);
        }

        public static EventFlyGraphQlBuilder ConfigureGraphQl(this EventFlyBuilder builder, Action<EventFlyGraphQlOptions> optionsBuilder)
        {
            var options = new EventFlyGraphQlOptions("graphql");

            var services = builder.Services;
            services.AddSingleton(options);
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddSingleton<Root>();
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
                services.AddSingleton(handlerFullType, handlerType);
                //services.AddSingleton(provider => (IGraphQueryHandler) provider.GetService(handlerFullType));
            }

            return new EventFlyGraphQlBuilder(builder);
        }

        public static EventFlyBuilder WithConsole(this EventFlyGraphQlBuilder builder, Action<EventFlyGraphQlConsoleOptions> builderOptions)
        {
            var options = new EventFlyGraphQlConsoleOptions("graphql-console");
            builderOptions(options);
            var services = builder.Services;
            services.AddSingleton(options);

            return builder.Builder;
        }

        //public static IApplicationBuilder UseEventFlyGraphQl(this IApplicationBuilder app)
        //{
        //    var options = app.ApplicationServices.GetRequiredService<EventFlyGraphQlOptions>();
        //    var optionsConsole = app.ApplicationServices.GetRequiredService<EventFlyGraphQlConsoleOptions>();


        //    app.UseGraphQL<ISchema>("/" + options.BasePath.Trim('/'));
        //    app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
        //    {
        //        Path = "/" + optionsConsole.BasePath.Trim('/')
        //    });
        //    return app;
        //}
    }
}
