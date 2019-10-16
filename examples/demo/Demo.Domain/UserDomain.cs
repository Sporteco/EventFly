using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.AggregateStorages;
using Akkatecture.Definitions;
using Akkatecture.Storages.EntityFramework;
using Demo.Commands;
using Demo.Domain.Aggregates;
using Demo.Domain.QueryHandlers;
using Demo.Events;
using Demo.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Domain
{
    public class UserDomain : DomainDefinition
    {
        public UserDomain(ActorSystem system) : base(system)
        {

            RegisterAggregate<UserAggregate, UserId>();
            RegisterSaga<TestSaga, TestSagaId>();
            RegisterCommand<CreateUserCommand>();
            RegisterCommand<RenameUserCommand>();
            RegisterQuery<UsersQueryHandler,UsersQuery,UsersResult>();
            RegisterQuery<EventPostersQueryHandler,EventPostersQuery,EventPosters>();
            RegisterEvents(typeof(UserCreatedEvent), typeof(UserRenamedEvent));
        }
    }

}