using Akka.Actor;
using Demo.Infrastructure;
using EventFly.DependencyInjection;
using EventFly.Web;
using EventFly.Web.GraphQL;
using EventFly.Web.Swagger;
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
                .AddSingleton("AAAA");

            services
                .AddEventFly(system)
                    .WithContext<UserContext>()
                    .AddGraphQl()
                    .AddSwagger()
                .BuildEventFly();

            services
                .AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseEventFly();

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
