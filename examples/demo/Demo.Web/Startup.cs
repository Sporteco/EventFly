using Akka.Actor;
using EventFly.AggregateStorages;
using EventFly.Definitions;
using EventFly.DependencyInjection;
using EventFly.Storages.EntityFramework;
using EventFly.Web;
using EventFly.Web.GraphQL;
using EventFly.Web.Swagger;
using Demo.Db;
using Demo.Dependencies;
using Demo.Domain;
using Demo.Domain.Aggregates;
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

            services
                    .AddSingleton("AAAA")
                    .AddScoped<TestSaga>()
                    .AddScoped<IAggregateStorage<UserAggregate>, EntityFrameworkAggregateStorage<UserAggregate, TestDbContext>>()
                    .AddScoped<EntityFrameworkAggregateStorage<UserAggregate, TestDbContext>>()
                        .AddEventFly(
                            system,
                            b => 
                                b.RegisterDomainDefinitions<UserDomain>().WithDependencies<UserDomainDependencies>()
                        )
                        .AddEventFlyGraphQl()
                        .AddEventFlySwagger();

            /*services.AddTransient<EnumerationGraphType<StringOperator>>();
            services.AddTransient<EnumerationGraphType<CollectionOperator>>();
            services.AddTransient<EnumerationGraphType<DateTimeOperator>>();
            services.AddTransient<EnumerationGraphType<NumericOperator>>();
            services.AddTransient<EnumerationGraphType<UriHostNameType>>();*/

            services.AddMvcCore().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<EventFlyMiddleware>();

            app.UseEventFlyDependencyInjection();
            app.UseEventFlyGraphQl();
            app.UseEventFlySwagger();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
