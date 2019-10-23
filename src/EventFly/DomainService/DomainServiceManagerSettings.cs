using Akka.Configuration;
using EventFly.Configuration;

namespace EventFly.DomainService
{
    public class DomainServiceManagerSettings
    {
        private static readonly string _section = "EventFly.aggregate-domain-service-manager";
        public readonly bool AutoSubscribe;
        public readonly bool AutoSpawnOnReceive;
        public DomainServiceManagerSettings(Config config)
        {
            var aggregateSagaManagerConfig = config.WithFallback(EventFlyDefaultSettings.DefaultConfig());
            aggregateSagaManagerConfig = aggregateSagaManagerConfig.GetConfig(_section);

            AutoSpawnOnReceive = aggregateSagaManagerConfig.GetBoolean("auto-spawn-on-receive");
            AutoSubscribe = aggregateSagaManagerConfig.GetBoolean("auto-subscribe");
        }
    }
}