using Akka.Actor;
using Akkatecture;
using Akkatecture.Aggregates;
using Akkatecture.AggregateStorages;
using Akkatecture.Definitions;
using Akkatecture.DependencyInjection;
using Akkatecture.Storages.EntityFramework;
using Akkatecture.Web;
using Akkatecture.Web.GraphQL;
using Akkatecture.Web.Swagger;
using Demo.Db;
using Demo.Domain;
using Demo.Domain.Aggregates;
using Demo.Predicates;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Web
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
            //Create actor system
            var system = ActorSystem.Create("user-example");

            services.AddSingleton("AAAA")
                    .AddScoped<TestSaga>()
                    .AddScoped<IAggregateStorage<UserAggregate>, EntityFrameworkStorage<UserAggregate, TestDbContext>>()
                    .AddScoped<EntityFrameworkStorage<UserAggregate, TestDbContext>>()
                    .AddAkkatecture(
                        system,
                        b => b.RegisterDomainDefinitions<UserDomain>()
                    );

            // DO not inejct
            system.RegisterDependencyResolver(services.BuildServiceProvider());

            var applicationDef = system.GetExtension<ServiceProviderHolder>();

            services.AddAkkatectureGraphQl(applicationDef.ServiceProvider.GetService<IApplicationDefinition>());
            services.AddAkkatectureSwagger();

            services.AddTransient<EnumerationGraphType<StringOperator>>();

            services.AddMvcCore().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<AkkatectureMiddleware>();

            app.UseAkkatectureGraphQl();
            app.UseAkkatectureSwagger();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
