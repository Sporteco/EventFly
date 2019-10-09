using Akka.Actor;

namespace Akkatecture.AggregateStorages
{
    public class AggregateStorageExtensionIdProvider : ExtensionIdProvider<AggregateStorageExtension>
    {
        public override AggregateStorageExtension CreateExtension(ExtendedActorSystem system)
        {
            return new AggregateStorageExtension();
        }

        public static AggregateStorageExtensionIdProvider Instance = new AggregateStorageExtensionIdProvider();
    }
}