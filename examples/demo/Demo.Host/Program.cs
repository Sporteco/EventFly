using System;
using System.Threading.Tasks;
using EventFly.DependencyInjection;
using Demo.Commands;
using Demo.Queries;
using Demo.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using EventFly.Queries;
using EventFly.Commands;
using System.Linq;
using Demo.Infrastructure;

namespace Demo.Host
{
    public static class Program
    {
        public static async Task Main()
        {
            var aggregateId = UserId.New;
            var createUserAccountCommand = new CreateUserCommand(aggregateId, new UserName(("userName","ru")), new Birth(DateTime.Now));

            var serviceProvider =
                 new ServiceCollection()
                     .AddSingleton("aa")

                     .AddEventFly("user-example")
                        .WithContext<UserContext>()
                     .Services
                    .BuildServiceProvider();

            serviceProvider
                .UseEventFly();



            var bus = serviceProvider.GetRequiredService<ICommandBus>();
            var queryProcessor = serviceProvider.GetRequiredService<IQueryProcessor>();

            await bus.Publish(createUserAccountCommand);

            await bus.Publish(new RenameUserCommand(aggregateId, new UserName(("TEST","en"))));
            await bus.Publish(new CreateUserCommand(UserId.New, new UserName(("userName2","ru")), new Birth(DateTime.Today)));

            var res = await queryProcessor.Process(new UsersQuery());
            Console.WriteLine(res?.Items.First().Name);

            //block end of program
            Console.ReadLine();

            Console.ReadLine();
        }
    }
}
