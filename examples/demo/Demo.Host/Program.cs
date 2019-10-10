using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture;
using Akkatecture.AggregateStorages;
using Akkatecture.Storages.EntityFramework;
using Autofac;
using Demo.Commands;
using Demo.Db;
using Demo.Db.QueryHandlers;
using Demo.Domain;
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

            system
                .RegisterAggregate<UserAggregate, UserId>()
                .RegisterQuery<UsersQueryHandler, UsersQuery, ICollection<UserInfo>>();

            var result = await system.PublishCommandAsync(createUserAccountCommand);
            
            result = await system.PublishCommandAsync(new RenameUserCommand(aggregateId, new UserName("TEST")));

            var res = await system.QueryAsync(new UsersQuery("test"));
                        
            //block end of program
            Console.ReadLine();
        }
    }
}
