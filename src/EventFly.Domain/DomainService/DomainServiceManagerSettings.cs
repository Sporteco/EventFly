using Akka.Configuration;
using EventFly.Configuration;

namespace EventFly.DomainService
{
    public class DomainServiceManagerSettings
    {
        public DomainServiceManagerSettings(Config config)
        {
            var domainServiceManagerConfig = config.WithFallback(EventFlyDefaultSettings.DefaultConfig());
            domainServiceManagerConfig = domainServiceManagerConfig.GetConfig(_section);

            AutoSpawnOnReceive = domainServiceManagerConfig.GetBoolean("auto-spawn-on-receive");
            AutoSubscribe = domainServiceManagerConfig.GetBoolean("auto-subscribe");
        }

        public bool AutoSubscribe { get; }
        public bool AutoSpawnOnReceive { get; }

        private const string _section = "EventFly.domain-service-manager";
    }
}