using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akkatecture;
using Akkatecture.AggregateStorages;
using Akkatecture.Storages.EntityFramework;
using Akkatecture.Web.Swagger;
using Demo.Db;
using Demo.Domain;
using Demo.Domain.Aggregates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using Info = Swashbuckle.AspNetCore.Swagger.Info;

namespace Demo.Ewb3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var system = ActorSystem.Create("api-system");

            system
                .AddAggregateStorageFactory()
                .RegisterDefaultStorage<InMemoryAggregateStorage>()
                .RegisterStorage<UserAggregate,EntityFrameworkStorage<TestDbContext>>();

            system
                .RegisterDomain<UserDomain>();

            // Add Actors to DI as ActorRefProvider<T>
            services
                .AddAkkatecture(system);

            AddSwagger(services);        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            UseSwagger(app);


        }
        public static IServiceCollection AddSwagger(IServiceCollection services)
        {
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
                c.SwaggerDoc("v1", new Info());
                //c.OperationFilter<AuthorizeCheckOperationFilter>();
                //c.OperationFilter<ApplySwaggerOperationAttributes>();
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
            return services;
        }
        public static IApplicationBuilder UseSwagger(IApplicationBuilder app)
        {

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            SwaggerBuilderExtensions.UseSwagger(app);

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sporteco");
                //c.OAuthClientId("swaggerui");
                //c.OAuthAppName("Swagger UI");
            });
            return app;
        }
    }
}