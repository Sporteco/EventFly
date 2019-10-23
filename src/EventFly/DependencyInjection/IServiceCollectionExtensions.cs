using Akka.Actor;
using EventFly.Commands;
using EventFly.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static EventFlyBuilder AddEventFly(
            this IServiceCollection services,
            ActorSystem actorSystem)
        {

            services.AddSingleton(actorSystem);

            services.AddTransient<ISerializedCommandPublisher, SerializedCommandPublisher>();
            services.AddTransient<ISerializedQueryExecutor, SerializedQueryExecutor>();

            services.AddTransient<ICommandBus, CommandToAggregateManagerBus>();
            services.AddTransient<IQueryProcessor, QueryToQueryHandlerProcessor>();



            return new EventFlyBuilder(actorSystem, services);
        }
    }
}
