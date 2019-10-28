﻿using System;
using System.Threading.Tasks;
using Akka.Actor;
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
            var createUserAccountCommand = new CreateUserCommand(aggregateId, new UserName("userName"), new Birth(DateTime.Now));

            //Create actor system
            var system = ActorSystem.Create("user-example");

            var serviceProvider =
                 new ServiceCollection()
                     .AddSingleton("aa")

                     .AddEventFly(system)
                        .WithContext<UserContext>()
                     .Services
                    .BuildServiceProvider();

            serviceProvider
                .UseEventFly();


            var holder = system.GetExtension<ServiceProviderHolder>();

            var bus = holder.ServiceProvider.GetRequiredService<ICommandBus>();
            var queryProcessor = holder.ServiceProvider.GetRequiredService<IQueryProcessor>();

            await bus.Publish(createUserAccountCommand);

            await bus.Publish(new RenameUserCommand(aggregateId, new UserName("TEST")));
            await bus.Publish(new CreateUserCommand(UserId.New, new UserName("userName2"), new Birth(DateTime.Today)));

            var res = await queryProcessor.Process(new UsersQuery());
            Console.WriteLine(res?.Items.First().Name);

            //block end of program
            Console.ReadLine();

            system.Dispose();

            Console.ReadLine();
        }
    }
}
