using Akka.Actor;

namespace EventFly.DependencyInjection
{
    public class ServiceProviderExtension : ExtensionIdProvider<ServiceProviderHolder>
    {
        public ServiceProviderExtension(ServiceProviderHolder serviceProviderHolder)
        {
            _serviceProviderHolder = serviceProviderHolder;
        }

        public override ServiceProviderHolder CreateExtension(ExtendedActorSystem system)
        {
            return _serviceProviderHolder;
        }

        private readonly ServiceProviderHolder _serviceProviderHolder;
    }
}