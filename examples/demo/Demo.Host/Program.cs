using System;
using System.Threading.Tasks;
using Akka.Actor;
using EventFly.DependencyInjection;
using Demo.Commands;
using Demo.Db;
using Demo.Domain;
using Demo.Queries;
using Demo.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EventFly.Definitions;
using Demo.Dependencies;
using EventFly.Queries;
using EventFly.Commands;

namespace Demo.Host
{
    public static class Program
    {
        public static async Task Main()
        {
            var aggregateId = UserId.New;
            var createUserAccountCommand = new CreateUserCommand(aggregateId, new UserName("userName"), new Birth(DateTime.Now));

            //Create actor system
            var system = ActorSystem.Create("user-example");

            var serviceCollection =
                new ServiceCollection()
                    .AddSingleton("AAAA")
                    .AddEventFly(
                        system,
                        domainBuilder =>
                        {
                            domainBuilder
                                .RegisterDomainDefinitions<UserDomain>()
                                .WithDependencies<UserDomainDependencies>();
                        });

            system.RegisterDependencyResolver(serviceCollection.BuildServiceProvider());

            // migrations todo
            using (var st = new TestDbContext())
            {
                try
                {
                    st.Database.Migrate();
                }
                catch (Exception) { }
            }

            var holder = system.GetExtension<ServiceProviderHolder>();

            var bus = holder.ServiceProvider.GetRequiredService<ICommandBus>();
            var queryProcessor = holder.ServiceProvider.GetRequiredService<IQueryProcessor>();


            await bus.Publish(createUserAccountCommand);

            //await system.PublishCommandAsync(new RenameUserCommand(aggregateId, new UserName("TEST")));
            //await system.PublishCommandAsync(new CreateUserCommand(UserId.New, new UserName("userName2"),new Birth(DateTime.Today)));

            var res = await queryProcessor.Process(new UsersQuery());
            Console.WriteLine(res?.Items.Count);

            //block end of program
            Console.ReadLine();

            system.Dispose();

            Console.ReadLine();
        }
    }
}
