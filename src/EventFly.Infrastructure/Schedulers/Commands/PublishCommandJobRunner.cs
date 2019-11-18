using EventFly.Commands;
using EventFly.DependencyInjection;
using EventFly.Jobs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace EventFly.Schedulers.Commands
{
    public sealed class PublishCommandJobRunner : JobRunner<PublishCommandJob, PublishCommandJobId>,
        IRunAsync<PublishCommandJob, PublishCommandJobId>

    {
        private readonly IServiceProvider _serviceProvider;
        private IServiceScope _scope;

        public PublishCommandJobRunner()
        {
            _serviceProvider = Context.System.GetExtension<ServiceProviderHolder>().ServiceProvider;
        }

        public async Task<Boolean> RunAsync(PublishCommandJob job)
        {
            var bus = _scope.ServiceProvider.GetRequiredService<ICommandBus>();

            await bus.Publish(job.Command);

            return true;
        }

        protected override void PreStart()
        {
            base.PreStart();

            _scope = _serviceProvider.CreateScope();
        }

        protected override void PostStop()
        {
            base.PostStop();
            _scope.Dispose();
        }
    }
}
