using Akka.Util;
using EventFly.Commands;
using EventFly.Queries;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using EventFly.Infrastructure.DependencyInjection;

namespace EventFly.Swagger
{
    public sealed class EventFlySwaggerOptions
    {
        public EventFlySwaggerOptions(String url, String name)
        {
            Url = url;
            Name = name;
        }

        public String Url { get; set; }
        public String Name { get; set; }
    }

    public static class BuilderExtensions
    {
        public static EventFlyBuilder WithSwagger(this EventFlyWebApiBuilder builder, Action<EventFlySwaggerOptions> optionsBuilder)
        {
            var options = new EventFlySwaggerOptions("swagger", Assembly.GetEntryAssembly()?.GetName().Name);
            optionsBuilder(options);
            builder.Services.AddSingleton(options);

            var services = builder.Services;
            services.TryAdd(ServiceDescriptor.Transient<IApiDescriptionGroupCollectionProvider, CommandsApiDescriptionGroupCollectionProvider>());

            services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (apiDesc.TryGetMethodInfo(out _))
                    {
                        return apiDesc.HttpMethod != null;
                    }
                    return false;
                });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = options.Name + " API", Version = "v1" });


                c.OperationFilter<DescriptionFilter>();
                c.SchemaFilter<ReadOnlyFilter>();
                c.CustomSchemaIds(i => i.FullName);
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var files = Directory.GetFiles(basePath, "*.xml");
                foreach (var file in files)
                {
                    c.IncludeXmlComments(file);
                }
                //c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                //{
                //	Type = "oauth2",
                //	Flow = "implicit",
                //	AuthorizationUrl = $"{siteOptions?.Security?.BaseUrl}/connect/authorize",
                //	TokenUrl = $"{siteOptions?.Security?.BaseUrl}/connect/token",
                //	Scopes = currentScopes
                //});
            });
            return builder.Builder;
        }

        //public static IApplicationBuilder UseEventFlySwagger(this IApplicationBuilder app)
        //{

        //    app.UseSwagger();
        //    var options = app.ApplicationServices.GetRequiredService<EventFlySwaggerOptions>();
        //    app.UseSwaggerUI(c =>
        //    {
        //        c.SwaggerEndpoint("/" + options.Url.Trim('/') + "/v1/swagger.json", options.Name);

        //        //c.OAuthClientId("swaggerui");
        //        //c.OAuthAppName("Swagger UI");
        //    });
        //    return app;
        //}

    }

    public class DescriptionFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor desc)
            {
                var actionType = desc.ControllerTypeInfo.AsType();
                if (actionType.Implements(typeof(ICommand)) || actionType.Implements(typeof(IQuery)))
                {
                    operation.Summary = actionType.GetCustomAttribute<DescriptionAttribute>()?.Description;
                }
            }
        }
    }
    public class ReadOnlyFilter : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            if (model.Properties == null)
            {
                return;
            }

            foreach (var schemaProperty in model.Properties)
            {
                var property = context.SystemType.GetProperty(schemaProperty.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    schemaProperty.Value.ReadOnly = false;
                }
            }
        }
    }
}
