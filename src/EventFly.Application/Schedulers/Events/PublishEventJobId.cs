using EventFly.Core;
using EventFly.Jobs;

namespace EventFly.Schedulers.Events
{
    public sealed class PublishEventJobId : Identity<PublishEventJobId>, IJobId
    {
        public PublishEventJobId(System.String value)
            : base(value)
        {
        }
    }
}
