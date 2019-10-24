using Akka.Actor;

namespace EventFly.DependencyInjection
{
    public class ServiceProviderExtension : ExtensionIdProvider<ServiceProviderHolder>
    {
        private readonly ServiceProviderHolder _serviceProviderHolder;

        public ServiceProviderExtension(ServiceProviderHolder serviceProviderHolder)
        {
            _serviceProviderHolder = serviceProviderHolder;
        }

        public override ServiceProviderHolder CreateExtension(ExtendedActorSystem system)
        {
            return _serviceProviderHolder;
        }
    }
}
