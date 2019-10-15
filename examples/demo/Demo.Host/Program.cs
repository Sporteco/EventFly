using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture;
using Akkatecture.DependencyInjection;

using Akkatecture.AggregateStorages;
using Akkatecture.Storages.EntityFramework;
using Demo.Commands;
using Demo.Db;
using Demo.Domain;
using Demo.Domain.Aggregates;
using Demo.Queries;
using Demo.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Host
{
    class Program
    {
        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            
            var aggregateId = UserId.New;
            var createUserAccountCommand = new CreateUserCommand(aggregateId, new UserName("userName"),new Birth(DateTime.Now));
            
            //Create actor system
            var system = ActorSystem.Create("user-example");

            var serviceCollection = 
                new ServiceCollection()
                    .AddSingleton("AAAA")
                    .AddScoped<TestSaga>();
            
            system.UseIoC(serviceCollection.BuildServiceProvider());

            // migrations todo
            using (var st = new TestDbContext())
            {
                try
                {
                    st.Database.Migrate();
                }
                catch (Exception) { }
            }

            system
                .AddAggregateStorageFactory()
                .RegisterDefaultStorage<InMemoryAggregateStorage>()
                .RegisterStorage<UserAggregate, EntityFrameworkStorage<TestDbContext>>();

            system.RegisterDomain<UserDomain>();

            await system.PublishCommandAsync(createUserAccountCommand);
            
            //await system.PublishCommandAsync(new RenameUserCommand(aggregateId, new UserName("TEST")));
            //await system.PublishCommandAsync(new CreateUserCommand(UserId.New, new UserName("userName2"),new Birth(DateTime.Today)));

            var res = await system.ExecuteQueryAsync(new UsersQuery());
            Console.WriteLine(res?.Items.Count);
            
            //block end of program
            Console.ReadLine();

            system.Dispose();

            Console.ReadLine();
        }
    }
}
