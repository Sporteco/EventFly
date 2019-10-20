﻿using EventFly.AggregateStorages;
using EventFly.Definitions;
using Demo.Domain;
using Demo.Domain.Aggregates;
using Demo.Domain.QueryHandlers;
using Demo.Domain.ReadModels;
using EventFly.ReadModels;
using Microsoft.Extensions.DependencyInjection;
using EventFly.Queries;
using Demo.Queries;

namespace Demo.Dependencies
{
    public class UserDomainDependencies : IDomainDependencies<UserDomain>
    {
        public IServiceCollection Dependencies =>
            new ServiceCollection()
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
