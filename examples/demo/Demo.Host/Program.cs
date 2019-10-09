using System;
using Akka.Actor;
using Demo.Commands;
using Demo.Domain;
using Demo.ValueObjects;

namespace Demo.Host
{
    class Program
    {
        static void Main()
        {
            //Create actor system
            var system = ActorSystem.Create("user-example");

            //Create supervising aggregate manager for UserAccount aggregate root actors
            var aggregateManager = system.ActorOf(Props.Create(() => new UserAggregateManager()));

            //Build create user account aggregate command
            var aggregateId = UserId.New;
            var createUserAccountCommand = new CreateUserCommand(aggregateId, new UserName("userName"),new Birth(DateTime.Now));
            
            //Send command, this is equivalent to command.publish() in other cqrs frameworks
            aggregateManager.Tell(createUserAccountCommand);
                        
            //block end of program
            Console.ReadLine();
        }
    }
}
