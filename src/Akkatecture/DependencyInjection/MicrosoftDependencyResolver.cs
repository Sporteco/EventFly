using Akka.Actor;
using Akka.DI.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Akkatecture.DependencyInjection
{
    /// <summary>
    /// Defines services used by the Akka.Actor.ActorSystem extension system to create actors using Microsoft.Extensions.DependencyInjection
    /// </summary>
    public class MicrosoftDependencyResolver : IDependencyResolver
    {
        private readonly ConcurrentDictionary<string, Type> _typeCache;
        private readonly ConditionalWeakTable<ActorBase, IServiceScope> _references;
        private readonly ActorSystem _system;
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// Create a new <see cref="MicrosoftDependencyResolver"/>.
        /// </summary>
        /// <param name="scopeFactory">
        ///   A factory for actor-level dependency injection scopes.
        /// </param>
        /// <param name="system">
        /// The <see cref="ActorSystem"/> for which dependency resolution is being provided.
        /// </param>
        public MicrosoftDependencyResolver(IServiceScopeFactory scopeFactory, ActorSystem system)
        {
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

            _typeCache = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            _references = new ConditionalWeakTable<ActorBase, IServiceScope>();
        }

        /// <summary>
        /// Retrieves an actor's type with the specified name.
        /// </summary>
        /// <param name="actorName">The name of the actor to retrieve</param>
        /// <returns> The type with the specified actor name <see cref="T:System.Type" />.</returns>
        public Type GetType(string actorName)
        {
            if (string.IsNullOrWhiteSpace(actorName))
                throw new ArgumentException("Argument cannot be null, empty, or entirely composed of whitespace: 'actorName'.", nameof(actorName));

            var typeValue = actorName.GetTypeValue();
            _typeCache.TryAdd(actorName, typeValue);
            return _typeCache[actorName];
        }

        /// <summary>
        /// Generate <see cref="Akka.Actor.Props" /> to create an actor of the specified type.
        /// </summary>
        /// <typeparam name="TActor">
        /// The type of actor to create.
        /// </typeparam>
        /// <returns>
        /// The configured <see cref="Akka.Actor.Props" />.
        /// </returns>
        public Props Create<TActor>()
            where TActor : ActorBase
        {
            return Create(typeof(TActor));
        }

        /// <summary>
        ///     Generate <see cref="Akka.Actor.Props" /> to create an actor of the specified type.
        /// </summary>
        /// <param name="actorType">
        ///     The type of actor to create.
        /// </param>
        /// <returns>
        ///     The configured <see cref="Akka.Actor.Props" />.
        /// </returns>
        public Props Create(Type actorType)
        {
            var props = _system.GetExtension<DIExt>().Props(actorType);
            return props;
        }

        /// <summary>
        /// Create a factory delegate for creating instances of the specified actor type.
        /// </summary>
        /// <param name="actorType">
        /// The type of actor to create.
        /// </param>
        /// <returns>
        /// A delegate that returns new instances of the actor.
        /// </returns>
        public Func<ActorBase> CreateActorFactory(Type actorType)
        {
            if (actorType == null)
                throw new ArgumentNullException(nameof(actorType));

            return () =>
            {
                var scope = _scopeFactory.CreateScope();
                var actorRef = scope.ServiceProvider.GetService(actorType);
                var actor = actorRef as ActorBase;

                _references.Add(actor, scope);

                return actor;
            };
        }

        /// <summary>
        /// Release (dispose) the dependency-injection scope associated with the specified actor.
        /// </summary>
        /// <param name="actor">
        /// The target actor.
        /// </param>
        public void Release(ActorBase actor)
        {
            if (actor == null)
                throw new ArgumentNullException(nameof(actor));

            if (_references.TryGetValue(actor, out var scope))
            {
                scope.Dispose();
                _references.Remove(actor);
            }
        }
    }
}
