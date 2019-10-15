using Akka.Actor;
using Akka.DI.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Akkatecture.DependencyInjection
{
    public static class ActorSystemExtensions
    {
        public static ActorSystem UseIoC(
            this ActorSystem actorSystem,
            IServiceProvider serviceProvider)
        {
            var factoryScope = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            var resolver = new MicrosoftDependencyResolver(factoryScope, actorSystem);
            actorSystem.AddDependencyResolver(resolver);

            return actorSystem;
        }
    }
}
