using EventFly.Aggregates;
using EventFly.Jobs;

namespace EventFly.Schedulers.Events
{
    public sealed class PublishEventJob : Job<PublishEventJobId>
    {
        public PublishEventJob(PublishEventJobId jobId, IDomainEvent domainEvent) : base(jobId)
        {
            DomainEvent = domainEvent;
        }

        public IDomainEvent DomainEvent { get; }
    }
}
