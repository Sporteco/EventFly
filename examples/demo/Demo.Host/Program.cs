using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture;
using Akkatecture.AggregateStorages;
using Akkatecture.Storages.EntityFramework;
using Autofac;
using Demo.Commands;
using Demo.Db;
using Demo.Domain;
using Demo.Domain.Aggregates;
using Demo.Queries;
using Demo.ValueObjects;

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
        // Create and build your container
            var builder = new ContainerBuilder();

            var container = builder.Build();


            var aggregateId = UserId.New;
            var createUserAccountCommand = new CreateUserCommand(aggregateId, new UserName("userName"),new Birth(DateTime.Now));
            
            
            //Create actor system
            var system = ActorSystem.Create("user-example");
            system.UseAutofac(container);

            system
                .AddAggregateStorageFactory()
                .RegisterDefaultStorage<InMemoryAggregateStorage>()
                .RegisterStorage<UserAggregate,EntityFrameworkStorage<TestDbContext>>();

            system.RegisterDomain<UserDomain>();

            await system.PublishCommandAsync(createUserAccountCommand);
            
            //await system.PublishCommandAsync(new RenameUserCommand(aggregateId, new UserName("TEST")));

            //await system.PublishCommandAsync(new CreateUserCommand(UserId.New, new UserName("userName2"),new Birth(DateTime.Today)));

            var res = await system.ExecuteQueryAsync(new UsersQuery());
            Console.WriteLine(res?.Items.Count);
                        
            //block end of program
            Console.ReadLine();
        }
    }
}
