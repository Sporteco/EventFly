using Akka.Actor;
using EventFly.Commands;
using EventFly.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.TestFixture.Aggregates
{
    public static class IServiceCollectionExtensions
    {
        public static EventFlyBuilder AddTestEventFly(
            this IServiceCollection services,
            ActorSystem actorSystem)
        {
            services.AddEventFly(actorSystem);

            return new EventFlyBuilder(services);
        }
    }
}