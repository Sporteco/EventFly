using Akka.Actor;
using EventFly.Commands;
using EventFly.Core;
using EventFly.Definitions;
using EventFly.Jobs;
using EventFly.Queries;
using EventFly.ReadModels;
using EventFly.Sagas;
using EventFly.Sagas.AggregateSaga;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

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
