using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Akka.Util;
using EventFly.Commands;
using EventFly.DependencyInjection;
using EventFly.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EventFly.Swagger
{
    public static class BuilderExtensions
    {
        public static EventFlyHttpBuilder AddSwagger(this EventFlyHttpBuilder builder)
        {
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
                c.SwaggerDoc("v1", new Info {Title = "R3 API", Version = "v1"});
                //c.OperationFilter<AuthorizeCheckOperationFilter>();
                //c.OperationFilter<OperationFilter>();
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
            return builder;
        }

        public static IApplicationBuilder UseEventFlySwagger(this IApplicationBuilder app)
        {

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "R3");
                
                //c.OAuthClientId("swaggerui");
                //c.OAuthAppName("Swagger UI");
            });
            return app;
        }

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
