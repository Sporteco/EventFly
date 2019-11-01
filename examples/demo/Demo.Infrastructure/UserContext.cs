using Demo.Application;
using Demo.Domain.Project;
using Demo.Domain.Project.Commands;
using Demo.Domain.Project.Events;
using Demo.Domain.Services;
using Demo.Domain.User;
using Demo.Infrastructure.QueryHandlers;
using Demo.Infrastructure.ReadModels;
using Demo.Queries;
using Demo.User.Commands;
using Demo.User.Events;
using EventFly.AggregateStorages;
using EventFly.Definitions;
using EventFly.Permissions;
using EventFly.Queries;
using EventFly.ReadModels;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Infrastructure
{
    public class UserContext : ContextDefinition
    {
        public UserContext()
        {
            RegisterPermission(DemoContext.CreateUser);
            RegisterPermission<UserId>(DemoContext.ChangeUser);
            RegisterPermission(DemoContext.TestPermission);
            RegisterPermission<UserId>(DemoContext.TestUserPermission);

            RegisterQuery<User1Query, UsersResult>();
            RegisterQuery<User2Query, UsersResult>();
            RegisterQuery<UsersQuery, UsersResult>();
            RegisterQuery<EventPostersQuery, EventPosters>();

            RegisterAggregate<UserAggregate, UserId>();
            RegisterAggregate<ProjectAggregate, ProjectId>();

            RegisterEvents(
                typeof(UserCreatedEvent),
                typeof(UserRenamedEvent),
                typeof(UserNotesChangedEvent),
                typeof(UserTouchedEvent),
                typeof(ProjectCreatedEvent),
                typeof(ProjectDeletedEvent),
                typeof(CreatedEvent),
                typeof(DeletedEvent)
            );

            RegisterCommands(
                typeof(CreateUserCommand),
                typeof(RenameUserCommand),
                typeof(ChangeUserNotesCommand),
                typeof(TrackUserTouchingCommand),
                typeof(CreateProjectCommand),
                typeof(DeleteProjectCommand),
                typeof(CreateCommand),
                typeof(DeleteCommand)
            );

            RegisterAggregateReadModel<UsersInfoReadModel, UserId>();
            RegisterReadModel<TotalUsersReadModel, TotalUsersReadModelManager>();

            RegisterSaga<TestSaga, TestSagaId>();
            RegisterDomainService<UserTouchTrackingService>();
        }

        public override IServiceCollection DI(IServiceCollection services)
        {
            return services
                .AddScoped<IAggregateStorage<UserAggregate>, InMemoryAggregateStorage<UserAggregate>>()
                .AddScoped<IAggregateStorage<ProjectAggregate>, InMemoryAggregateStorage<ProjectAggregate>>()
                .AddSingleton<IReadModelStorage<UsersInfoReadModel>, InMemoryReadModelStorage<UsersInfoReadModel>>()
                .AddSingleton<IReadModelStorage<TotalUsersReadModel>, InMemoryReadModelStorage<TotalUsersReadModel>>()

                .AddScoped<TestSaga>()

                .AddScoped<ReadModelHandler<TotalUsersReadModel>>()
                .AddScoped<ReadModelHandler<UsersInfoReadModel>>()

                .AddScoped<QueryHandler<UsersQuery, UsersResult>, UsersQueryHandler>()
                .AddScoped<QueryHandler<User1Query, UsersResult>, UsersQuery1Handler>()
                .AddScoped<QueryHandler<User2Query, UsersResult>, UsersQuery2Handler>()
                .AddScoped<QueryHandler<EventPostersQuery, EventPosters>, EventPostersQueryHandler>()

                .AddSingleton<IPermissionProvider, PermissionProvider>();
        }
    }
}