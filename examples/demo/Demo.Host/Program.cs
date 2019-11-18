using Demo.Infrastructure;
using Demo.User.Commands;
using Demo.ValueObjects;
using EventFly.Commands;
using EventFly.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Demo.Host
{
    public static class Program
    {
        public static async Task Main()
        {
            var aggregateId = UserId.New;
            var createUserAccountCommand = new CreateUserCommand(aggregateId, new UserName(("userName", "ru")), new Birth(DateTime.Now));

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
            //var queryProcessor = serviceProvider.GetRequiredService<IQueryProcessor>();

            await bus.Publish(createUserAccountCommand);

            await bus.Publish(new RenameUserCommand(aggregateId, new UserName(("TEST", "en"))));
            await bus.Publish(new CreateUserCommand(UserId.New, new UserName(("userName2", "ru")), new Birth(DateTime.Today)));

            var projectId1 = ProjectId.New;
            var projectId2 = ProjectId.New;

            var result1 = await bus.Publish(new CreateProjectCommand(aggregateId, projectId1, new ProjectName("Test project #1")));
            Console.WriteLine(result1.ToString());

            var result2 = await bus.Publish(new CreateProjectCommand(aggregateId, projectId2, new ProjectName("Test project #1")));
            Console.WriteLine(result2.ToString());

            await bus.Publish(new DeleteProjectCommand(aggregateId, projectId1));
            Console.WriteLine(result2.ToString());

            //var res = await queryProcessor.Process(new UsersQuery());
            //Console.WriteLine(res?.Items.First().Name);

            //block end of program
            Console.ReadLine();

            Console.ReadLine();
        }
    }
}
