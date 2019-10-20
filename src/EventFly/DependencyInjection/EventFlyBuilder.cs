using EventFly.Definitions;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.DependencyInjection
{
    public sealed class EventFlyBuilder
    {

        public EventFlyBuilder(IApplicationDefinition appDef, IServiceCollection services)
        {
            ApplicationDefinition = appDef;
            Services = services;
        }

        public IServiceCollection Services { get; }
        public IApplicationDefinition ApplicationDefinition { get; }
    }
}
