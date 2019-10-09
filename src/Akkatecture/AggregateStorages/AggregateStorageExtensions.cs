using Akka.Actor;
using Akkatecture.Aggregates;

namespace Akkatecture.AggregateStorages
{
    public class AggregateStoreBuilder
    {
        private readonly IAggregateStorageFactory _instance;

        public AggregateStoreBuilder(IAggregateStorageFactory instance)
        {
            _instance = instance;
        }

        public AggregateStoreBuilder RegisterStorage<TAggregate, TAggregateStorage>()
            where TAggregate : IAggregateRoot
            where TAggregateStorage : IAggregateStorage
        {
            _instance.RegisterStorage<TAggregate,TAggregateStorage>();
            return this;
        }

        public AggregateStoreBuilder RegisterDefaultStorage<TAggregateStorage>()
            where TAggregateStorage : IAggregateStorage 
        {
            _instance.RegisterDefaultStorage<TAggregateStorage>();
            return this;
        }
    }
    public static class AggregateStorageExtensions
    {
        public static AggregateStoreBuilder AddAggregateStorageFactory(this ActorSystem system, IAggregateStorageFactory factory)
        {
            system.RegisterExtension(AggregateStorageExtensionIdProvider.Instance);
            AggregateStorageExtensionIdProvider.Instance.Get(system).Initialize(factory);
            return new AggregateStoreBuilder(factory);
        }
        public static AggregateStoreBuilder AddAggregateStorageFactory(this ActorSystem system)
        {
            var factory = new AggregateStorageFactory();
            system.RegisterExtension(AggregateStorageExtensionIdProvider.Instance);
            AggregateStorageExtensionIdProvider.Instance.Get(system).Initialize(factory);
            return new AggregateStoreBuilder(factory);
        }

        public static IAggregateStorage GetAggregateStorage<TAggregate>(this IActorContext context)
            where TAggregate : IAggregateRoot
        {
            return AggregateStorageExtensionIdProvider.Instance.Get(context.System).GetAggregateStorage<TAggregate>();
        }
    }
}