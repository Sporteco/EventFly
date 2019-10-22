using Akka.Actor;
using EventFly.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;

namespace EventFly.Web
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseEventFly(this IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<EventFlyMiddleware>()
                .UseEventFlyDependencyInjection();
        }


        public static IApplicationBuilder UseEventFlyDependencyInjection(this IApplicationBuilder builder)
        {
            var serviceProvider =  builder.ApplicationServices;
            var actorSystem = (ActorSystem) serviceProvider.GetService(typeof(ActorSystem));

            actorSystem.RegisterDependencyResolver(serviceProvider);

            return builder;
        }
    }
}
