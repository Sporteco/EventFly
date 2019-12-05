using Demo.Application;
using Demo.Application.CreateProject;
using Demo.Application.DeleteProject;
using Demo.Application.RenameUser;
using Demo.Domain.Project;
using Demo.Domain.Project.Commands;
using Demo.Domain.Project.Events;
using Demo.Domain.Services;
using Demo.Domain.User;
using Demo.Infrastructure.AggregateStates;
using Demo.Infrastructure.CreateProject;
using Demo.Infrastructure.QueryHandlers;
using Demo.Infrastructure.ReadModels;
using Demo.Queries;
using Demo.User.Commands;
using Demo.User.Events;
using EventFly.Aggregates;
using EventFly.Definitions;
using EventFly.DependencyInjection;
using EventFly.Permissions;
using EventFly.Queries;
using EventFly.ReadModels;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

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
            RegisterQuery<TestQuery, IEnumerable<TestResult>>();
            RegisterQuery<EventPostersQuery, EventPosters>();

            RegisterAggregate<UserAggregate, UserId>();
            RegisterAggregate<ProjectAggregate, ProjectId>();

            RegisterPublicEvents(
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
                .AddScoped<IAggregateState<UserId>, UserState>()
                .AddScoped<IAggregateState<ProjectId>, ProjectStateInMemory>()
                .AddSingleton<IReadModelStorage<UsersInfoReadModel>, InMemoryReadModelStorage<UsersInfoReadModel>>()
                .AddSingleton<IReadModelStorage<TotalUsersReadModel>, InMemoryReadModelStorage<TotalUsersReadModel>>()

                .AddScoped<TestSaga>()

                .AddScoped<ReadModelHandler<TotalUsersReadModel>>()
                .AddScoped<ReadModelHandler<UsersInfoReadModel>>()

                .AddScoped<QueryHandler<UsersQuery, UsersResult>, UsersQueryHandler>()
                .AddScoped<QueryHandler<User1Query, UsersResult>, UsersQuery1Handler>()
                .AddScoped<QueryHandler<User2Query, UsersResult>, UsersQuery2Handler>()
                .AddScoped<QueryHandler<EventPostersQuery, EventPosters>, EventPostersQueryHandler>()

                .AddScoped<IExternalService, TestExternalService>()

                .AddCommandHandler<UserAggregate, UserId, RenameUserCommand, RenameUserCommandHandler>()
                .AddCommandHandler<UserAggregate, UserId, CreateProjectCommand, CreateProjectCommandHandler>()
                .AddCommandHandler<UserAggregate, UserId, DeleteProjectCommand, DeleteProjectCommandHandler>()

                .AddSingleton<IPermissionProvider, PermissionProvider>();
        }
    }
}