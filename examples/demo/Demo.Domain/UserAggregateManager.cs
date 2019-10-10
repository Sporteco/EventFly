using Akkatecture.Aggregates;

namespace Demo.Domain
{
    public class UserAggregateManager : AggregateManager<UserAggregate, UserId>{}
}