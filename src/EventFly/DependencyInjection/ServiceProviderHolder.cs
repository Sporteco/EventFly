using Akka.Actor;
using System;

namespace EventFly.DependencyInjection
{
    public class ServiceProviderHolder : IExtension
    {
        public ServiceProviderHolder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

    }
}
