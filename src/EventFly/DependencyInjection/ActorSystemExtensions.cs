using Akka.Actor;
using Akka.DI.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventFly.DependencyInjection
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
