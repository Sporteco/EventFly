using EventFly.AggregateStorages;
using EventFly.Definitions;
using EventFly.Storages.EntityFramework;
using Demo.Db;
using Demo.Domain;
using Demo.Domain.Aggregates;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Dependencies
{
    public class UserDomainDependencies : IDomainDependencies<UserDomain>
    {
        public IServiceCollection Dependencies =>
            new ServiceCollection()
                    .AddScoped<TestSaga>()
                    .AddScoped<IAggregateStorage<UserAggregate>, EntityFrameworkStorage<UserAggregate, TestDbContext>>();
    }
}
