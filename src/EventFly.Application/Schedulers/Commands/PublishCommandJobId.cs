using EventFly.Core;
using EventFly.Jobs;

namespace EventFly.Schedulers.Commands
{
    public sealed class PublishCommandJobId : Identity<PublishCommandJobId>, IJobId
    {
        public PublishCommandJobId(System.String value)
            : base(value)
        {
        }
    }
}
