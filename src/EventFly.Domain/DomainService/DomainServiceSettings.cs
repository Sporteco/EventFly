using Akka.Configuration;
using EventFly.Configuration;

namespace EventFly.DomainService
{
    public class DomainServiceSettings
    {
        private static string _section = "EventFly.domain-service";
        public readonly bool AutoReceive;
        public DomainServiceSettings(Config config)
        {
            var aggregateSagaConfig = config.WithFallback(EventFlyDefaultSettings.DefaultConfig());
            aggregateSagaConfig = aggregateSagaConfig.GetConfig(_section);

            AutoReceive = aggregateSagaConfig.GetBoolean("auto-receive");
        }
    }
}