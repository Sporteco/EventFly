using Demo.Application;
using Demo.Commands;
using Demo.Domain.Aggregates;
using Demo.Domain.Services;
using Demo.Events;
using Demo.Infrastructure.QueryHandlers;
using Demo.Infrastructure.ReadModels;
using Demo.Queries;
using EventFly.AggregateStorages;
using EventFly.Definitions;
using EventFly.Queries;
using EventFly.ReadModels;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Infrastructure
{
    public class UserContext : ContextDefinition
    {
        public UserContext()
        {
            RegisterQuery<User1Query, UsersResult>();
            RegisterQuery<User2Query, UsersResult>();
            RegisterQuery<UsersQuery, UsersResult>();
            RegisterQuery<EventPostersQuery, EventPosters>();

            RegisterAggregate<UserAggregate, UserId>();

            RegisterCommand<CreateUserCommand>();
            RegisterCommand<RenameUserCommand>();

            RegisterAggregateReadModel<UsersInfoReadModel, UserId>();

            RegisterReadModel<TotalUsersReadModel, TotalUsersReadModelManager>();

            RegisterSaga<TestSaga, TestSagaId>();

            RegisterEvents(typeof(UserCreatedEvent), typeof(UserRenamedEvent));

            RegisterDomainService<TestDomainService>();
        }

        public override IServiceCollection DI(IServiceCollection services)
        {
            return services
                .AddScoped<TestSaga>()
                .AddScoped<IAggregateStorage<UserAggregate>, InMemoryAggregateStorage<UserAggregate>>()
                .AddScoped<QueryHandler<UsersQuery, UsersResult>, UsersQueryHandler>()
                .AddScoped<QueryHandler<EventPostersQuery, EventPosters>, EventPostersQueryHandler>()
                .AddScoped<ReadModelHandler<TotalUsersReadModel>>()
                .AddScoped<ReadModelHandler<UsersInfoReadModel>>()
                .AddSingleton<IReadModelStorage<UsersInfoReadModel>, InMemoryReadModelStorage<UsersInfoReadModel>>()
                .AddSingleton<IReadModelStorage<TotalUsersReadModel>, InMemoryReadModelStorage<TotalUsersReadModel>>()
                .AddScoped<QueryHandler<User1Query, UsersResult>, UsersQuery1Handler>()
                .AddScoped<QueryHandler<User2Query, UsersResult>, UsersQuery2Handler>();
        }
    }
}