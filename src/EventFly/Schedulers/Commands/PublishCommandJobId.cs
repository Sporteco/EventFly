using EventFly.Core;
using EventFly.Jobs;

namespace EventFly.Schedulers.Commands
{
    public sealed class PublishCommandJobId : Identity<PublishCommandJobId>, IJobId
    {
        public PublishCommandJobId(string value)
            : base(value)
        {
        }
    }
}
