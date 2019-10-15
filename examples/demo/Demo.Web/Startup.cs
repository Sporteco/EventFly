using Akka.Actor;
using Akkatecture;
using Akkatecture.AggregateStorages;
using Akkatecture.Configuration.DependancyInjection;
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
            services.AddAkkatecture(system);

            system
                .AddAggregateStorageFactory()
                .RegisterDefaultStorage<InMemoryAggregateStorage>()
                .RegisterStorage<UserAggregate, EntityFrameworkStorage<TestDbContext>>();

            system.RegisterDomain<UserDomain>();

            services.AddAkkatectureGraphQl(system.GetApplicationDefinition());
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
