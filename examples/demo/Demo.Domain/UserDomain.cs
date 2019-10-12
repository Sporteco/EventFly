using System.Collections.Generic;
using Akka.Actor;
using Akkatecture.Definitions;
using Demo.Commands;
using Demo.Domain.Aggregates;
using Demo.Domain.QueryHandlers;
using Demo.Queries;

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
            RegisterQuery<UsersQueryHandler,UsersQuery,ICollection<UserInfo>>();
            
        }
    }
}