using EventFly.Definitions;
using Demo.Commands;
using Demo.Domain.Aggregates;
using Demo.Events;
using Demo.Queries;
using Microsoft.Extensions.DependencyInjection;
using EventFly.AggregateStorages;
using EventFly.Queries;
using EventFly.ReadModels;
using Demo.Infrastructure.ReadModels;
using Demo.Application;
using Demo.Infrastructure.QueryHandlers;

namespace Demo.Dependencies
{
    public class UserContext : ContextDefinition
    {
        public UserContext()
        {
            RegisterAggregate<UserAggregate, UserId>();
            RegisterCommand<CreateUserCommand>();
            RegisterCommand<RenameUserCommand>();
            RegisterQuery<UsersQuery, UsersResult>();
            RegisterQuery<EventPostersQuery, EventPosters>();
            RegisterAggregateReadModel<UsersInfoReadModel, UserId>();
            RegisterReadModel<TotalUsersReadModel, TotalUsersReadModelManager>();
            RegisterSaga<TestSaga, TestSagaId>();
            RegisterEvents(typeof(UserCreatedEvent), typeof(UserRenamedEvent));
        }

        public override IServiceCollection DI(IServiceCollection services) => services
            .AddScoped<TestSaga>()
            .AddScoped<IAggregateStorage<UserAggregate>, InMemoryAggregateStorage<UserAggregate>>()

            .AddScoped<QueryHandler<UsersQuery, UsersResult>, UsersQueryHandler>()
            .AddScoped<QueryHandler<EventPostersQuery, EventPosters>, EventPostersQueryHandler>()

            .AddScoped<ReadModelHandler<TotalUsersReadModel>>()
            .AddScoped<ReadModelHandler<UsersInfoReadModel>>()

            .AddSingleton<IReadModelStorage<UsersInfoReadModel>, InMemoryReadModelStorage<UsersInfoReadModel>>()
            .AddSingleton<IReadModelStorage<TotalUsersReadModel>, InMemoryReadModelStorage<TotalUsersReadModel>>();
    }
}