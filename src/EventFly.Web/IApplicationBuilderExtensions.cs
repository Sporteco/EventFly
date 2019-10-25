using EventFly.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace EventFly
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
    }
}
