using EventFly.DependencyInjection;
using EventFly.GraphQL;
using EventFly.Swagger;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;

namespace EventFly
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseEventFly(this IApplicationBuilder builder)
        {
            builder
                .UseMiddleware<EventFlyMiddleware>()
                .ApplicationServices
                .UseEventFly();


            if (builder.ApplicationServices.GetService(typeof(EventFlyGraphQlOptions)) is EventFlyGraphQlOptions optionsGraphQl)
                builder.UseGraphQL<ISchema>("/" + optionsGraphQl.BasePath.Trim('/'));

            if (builder.ApplicationServices.GetService(typeof(EventFlyGraphQlConsoleOptions)) is EventFlyGraphQlConsoleOptions optionsConsole)
                builder.UseGraphQLPlayground(new GraphQLPlaygroundOptions
                {
                    Path = "/" + optionsConsole.BasePath.Trim('/')
                });

            if (builder.ApplicationServices.GetService(typeof(EventFlySwaggerOptions)) is EventFlySwaggerOptions optionsSwagger)
            {
                builder.UseSwagger();
                builder.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/" + optionsSwagger.Url.Trim('/') + "/v1/swagger.json", optionsSwagger.Name);

                    //c.OAuthClientId("swaggerui");
                    //c.OAuthAppName("Swagger UI");
                });
            }


            return builder;
        }
    }
}
