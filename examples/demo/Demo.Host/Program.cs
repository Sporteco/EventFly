using System;
using Akka.Actor;
using Akkatecture.AggregateStorages;
using Akkatecture.Storages.EntityFramework;
using Autofac;
using Demo.Commands;
using Demo.Db;
using Demo.Domain;
using Demo.ValueObjects;

namespace Demo.Host
{
    class Program
    {
        static void Main()
        {
            // Create and build your container
            var builder = new ContainerBuilder();

            var container = builder.Build();
            
            
            //Create actor system
            var system = ActorSystem.Create("user-example");
            system.AddAggregateStorageFactory()
                .RegisterDefaultStorage<InMemoryAggregateStorage>()
                .RegisterStorage<UserAggregate,EntityFrameworkStorage<TestDbContext>>();

            system.UseAutofac(container);

            //Create supervising aggregate manager for UserAccount aggregate root actors
            var aggregateManager = system.ActorOf(Props.Create(() => new UserAggregateManager()));

            //Build create user account aggregate command
            var aggregateId = UserId.New;
            var createUserAccountCommand = new CreateUserCommand(aggregateId, new UserName("userName"),new Birth(DateTime.Now));
            
            //Send command, this is equivalent to command.publish() in other cqrs frameworks
            aggregateManager.Tell(createUserAccountCommand);
            var result = aggregateManager.Ask(new RenameUserCommand(aggregateId, new UserName("TEST"))).GetAwaiter().GetResult();
            Console.WriteLine(result);
                        
            //block end of program
            Console.ReadLine();
        }
    }
}
