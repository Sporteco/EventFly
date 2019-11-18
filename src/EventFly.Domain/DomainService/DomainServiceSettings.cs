using Akka.Configuration;
using EventFly.Configuration;

namespace EventFly.DomainService
{
    public class DomainServiceSettings
    {
        public DomainServiceSettings(Config config)
        {
            var domainServiceConfig = config.WithFallback(EventFlyDefaultSettings.DefaultConfig());
            domainServiceConfig = domainServiceConfig.GetConfig(_section);

            AutoReceive = domainServiceConfig.GetBoolean("auto-receive");
        }

        public System.Boolean AutoReceive { get; }

        private const System.String _section = "EventFly.domain-service";
    }
}