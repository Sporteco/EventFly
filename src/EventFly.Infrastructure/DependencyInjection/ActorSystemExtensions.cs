using System;
using Akka.Actor;
using Akka.DI.Core;
using EventFly.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.Infrastructure.DependencyInjection
{
    public static class ActorSystemExtensions
    {
        public static ActorSystem RegisterDependencyResolver(
            this ActorSystem actorSystem,
            IServiceProvider serviceProvider)
        {
            var factoryScope = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            var resolver = new MicrosoftDependencyResolver(factoryScope, actorSystem);
            actorSystem.AddDependencyResolver(resolver);

            actorSystem.RegisterExtension(new ServiceProviderExtension(new ServiceProviderHolder(serviceProvider)));
            return actorSystem;
        }
    }
}
