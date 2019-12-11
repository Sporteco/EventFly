using Demo.Infrastructure;
using EventFly;
using EventFly.GraphQL;
using EventFly.Infrastructure.DependencyInjection;
using EventFly.Swagger;
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
            services
                .AddSingleton("AAAA")
                .AddEventFly("user-example")
                    .WithContext<UserContext>()

                .ConfigureGraphQl(options => options.BasePath = "/graphql")
                    .WithConsole(options => options.BasePath = "/graphql-console")

                .ConfigureWebApi(options => options.BasePath = "/api-s")
                    .WithSwagger(options => options.Url = "/swagger");

            services
                .AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseEventFly();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
