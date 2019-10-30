using Akka.Configuration;
using EventFly.Configuration;
using System;

namespace EventFly.Domain
{
    public class DomainServiceSettings
    {
        public DomainServiceSettings(Config config)
        {
            var domainServiceConfig = config.WithFallback(EventFlyDefaultSettings.DefaultConfig());
            domainServiceConfig = domainServiceConfig.GetConfig(_section);

            AutoReceive = domainServiceConfig.GetBoolean("auto-receive");
        }

        public Boolean AutoReceive { get; }

        private const String _section = "EventFly.domain-service";
    }
}