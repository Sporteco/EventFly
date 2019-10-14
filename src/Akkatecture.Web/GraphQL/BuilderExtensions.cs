using Akkatecture.Definitions;
using GraphQL;
using GraphQL.Http;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Akkatecture.Web.GraphQL
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddAkkatectureGraphQl(this IServiceCollection services, IApplicationDefinition app)
        {
            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddScoped<GraphQueryInternal>();
            services.AddScoped<ISchema, GraphSchemaInternal>();
            services.AddGraphQL(_ =>
            {
                _.EnableMetrics = true;
                _.ExposeExceptions = true;
            });

            foreach (var query in app.Queries)
            {
                var handlerType = typeof(GraphQueryHandler<,>).MakeGenericType(query.Type, query.QueryResultType);
                services.AddScoped(typeof(IGraphQueryHandler),handlerType);
            }

            return services;
        }
        public static IApplicationBuilder UseAkkatectureGraphQl(this IApplicationBuilder app)
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
