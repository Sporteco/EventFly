using Akka.Actor;
using EventFly.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace EventFly.Web
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseEventFly(this IApplicationBuilder builder)
        {
            builder
                .UseMiddleware<EventFlyMiddleware>()
                .ApplicationServices
                .UseEventFly();

            return builder;
        }


        public static IApplicationBuilder UseEventFlyDependencyInjection(this IApplicationBuilder builder)
        {
            var serviceProvider = builder.ApplicationServices;
            var actorSystem = (ActorSystem)serviceProvider.GetService(typeof(ActorSystem));

            actorSystem.RegisterDependencyResolver(serviceProvider);

            return builder;
        }
    }
}
