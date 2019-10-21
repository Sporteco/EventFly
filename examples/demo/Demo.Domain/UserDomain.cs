using EventFly.Definitions;
using Demo.Commands;
using Demo.Domain.Aggregates;
using Demo.Domain.QueryHandlers;
using Demo.Domain.ReadModels;
using Demo.Events;
using Demo.Queries;

namespace Demo.Domain
{
    public class UserDomain : DomainDefinition
    {
        public UserDomain()
        {
            RegisterAggregate<UserAggregate, UserId>();
            RegisterCommand<CreateUserCommand>();
            RegisterCommand<RenameUserCommand>();
            RegisterQuery<UsersQuery, UsersResult>();
            RegisterQuery<EventPostersQuery, EventPosters>();

            RegisterEvents(typeof(UserCreatedEvent), typeof(UserRenamedEvent));
        }
    }

}