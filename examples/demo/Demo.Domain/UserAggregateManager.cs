using Akkatecture.Aggregates;
using Akkatecture.Commands;

namespace Demo.Domain
{
    public class UserAggregateManager : AggregateManager<UserAggregate, UserId, Command<UserId>>{}
}