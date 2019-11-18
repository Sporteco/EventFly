// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/EventFly 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Akka.Actor;
using EventFly.Definitions;
using EventFly.DependencyInjection;
using EventFly.Examples.Jobs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace EventFly.Examples.JobApplication
{
    public class Program
    {
        public static async Task Main(String[] args)
        {
            var actorSystem = ActorSystem.Create("print-job-system", Configuration.Config);

            var sp = new ServiceCollection()
            .AddEventFly(actorSystem)
                .WithContext<TestContext>()
                .Services
            .BuildServiceProvider();

            sp.UseEventFly();

            var scheduler = sp.GetService<EventFly.Jobs.IScheduler>();

            var firstJobId = PrintJobId.New;
            var secondJobId = PrintJobId.New;
            var thirdJobId = PrintJobId.New;

            var time = DateTime.UtcNow;

            await scheduler.Schedule<PrintJob, PrintJobId>(new PrintJob(firstJobId, "first"), TimeSpan.FromSeconds(5), time.AddSeconds(5));
            await scheduler.Schedule<PrintJob, PrintJobId>(new PrintJob(secondJobId, "second"), TimeSpan.FromSeconds(5), time.AddSeconds(6));
            await scheduler.Schedule<PrintJob, PrintJobId>(new PrintJob(thirdJobId, "THIRD"), time.AddSeconds(10));

            Console.ReadKey();

            await scheduler.Cancel<PrintJob, PrintJobId>(secondJobId);

            Console.ReadKey();

            Task shutdownTask = CoordinatedShutdown.Get(actorSystem)
                .Run(CoordinatedShutdown.ClrExitReason.Instance);

            await shutdownTask;
        }
    }

    public class TestContext : ContextDefinition
    {
        public TestContext()
        {
            RegisterJob<PrintJob, PrintJobId, PrintJobRunner, PrintJobScheduler>();
        }

        public override IServiceCollection DI(IServiceCollection serviceDescriptors)
        {
            return serviceDescriptors;
        }
    }
}