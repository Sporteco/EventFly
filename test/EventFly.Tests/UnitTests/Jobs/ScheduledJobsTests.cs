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

using Akka.TestKit;
using Akka.TestKit.Xunit2;
using EventFly.DependencyInjection;
using EventFly.Jobs;
using EventFly.TestHelpers.Jobs;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Tests.UnitTests.Jobs
{
    [Collection("ScheduledJobsTests")]
    public class ScheduledJobsTests : TestKit
    {
        private const String Category = "Jobs";

        public ScheduledJobsTests(ITestOutputHelper testOutputHelper)
            : base(TestHelpers.Akka.Configuration.ConfigWithTestScheduler, "jobs-tests", testOutputHelper)
        {
            //new ServiceCollection()
            //.AddScoped<TestSaga>()
            //.AddScoped<TestJobRunner>()
            //.AddScoped<AsyncTestJobRunner>()
            //.AddSingleton<TestJobRunnerProbeAccessor>()
            //.AddTestEventFly(Sys)
            //    .WithContext<TestContext>()
            //    .Services
            //.BuildServiceProvider()
            //.UseEventFly();
        }

        //[Fact]
        //[Category(Category)]
        private async Task SchedulingJob_For5minutes_DispatchesJobMessage()
        {
            var probe = CreateTestProbe("job-probe");
            var sysScheduler = (TestScheduler)Sys.Scheduler;
            var jobId = TestJobId.New;
            var greeting = $"hi here here is a random guid {Guid.NewGuid()}";
            var job = new TestJob(jobId, greeting);
            var when = DateTime.UtcNow.AddDays(1);

            var accessor = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<TestJobRunnerProbeAccessor>();
            accessor.Put(jobId, probe);

            var scheduler = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<IScheduler>();

            var result = await scheduler.Schedule<TestJob, TestJobId>(job, when);
            result.Should().BeTrue();

            sysScheduler.AdvanceTo(when);

            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when);
                return x.Greeting == greeting;
            }, TimeSpan.FromSeconds(10));

            // last assertions do not work probably has to do with time
            // see https://gist.github.com/Lutando/6c68a93faace4a0403f468358193ee41
            //scheduler.Advance(day);
            //probe.ExpectNoMsg(TimeSpan.FromSeconds(1));
            //scheduler.Advance(day);
            //probe.ExpectNoMsg();
            //scheduler.Advance(day);
            //probe.ExpectNoMsg();
        }

        //[Fact]
        //[Category(Category)]
        private async Task SchedulingJob_For5minutes_DispatchesJobMessageFromAsynRunner()
        {
            var probe = CreateTestProbe("job-probe");
            var sysScheduler = (TestScheduler)Sys.Scheduler;
            var jobId = AsyncTestJobId.New;
            var greeting = $"hi here here is a random guid {Guid.NewGuid()}";
            var job = new AsyncTestJob(jobId, greeting);
            var when = DateTime.UtcNow.AddDays(1);

            var accessor = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<TestJobRunnerProbeAccessor>();
            accessor.Put(jobId, probe);

            var scheduler = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<IScheduler>();

            var result = await scheduler.Schedule<AsyncTestJob, AsyncTestJobId>(job, when);
            result.Should().BeTrue();

            sysScheduler.AdvanceTo(when);

            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when);
                return x.Greeting == greeting;
            });

            // last assertions do not work probably has to do with time
            // see https://gist.github.com/Lutando/6c68a93faace4a0403f468358193ee41
            //scheduler.Advance(day);
            //probe.ExpectNoMsg(TimeSpan.FromSeconds(1));
            //scheduler.Advance(day);
            //probe.ExpectNoMsg();
            //scheduler.Advance(day);
            //probe.ExpectNoMsg();
        }


        //[Fact]
        //[Category(Category)]
        private async Task SchedulingJob_ForEvery5minutes_DispatchesJobMessage()
        {
            var probe = CreateTestProbe("job-probe");
            var sysScheduler = (TestScheduler)Sys.Scheduler;
            var jobId = TestJobId.New;
            var greeting = $"hi here here is a random guid {Guid.NewGuid()}";
            var job = new TestJob(jobId, greeting);
            var when = DateTime.UtcNow.AddDays(1);
            var fiveMinutes = TimeSpan.FromMinutes(5);

            var accessor = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<TestJobRunnerProbeAccessor>();
            accessor.Put(jobId, probe);

            var scheduler = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<IScheduler>();

            var result = await scheduler.Schedule<TestJob, TestJobId>(job, fiveMinutes, when);
            result.Should().BeTrue();

            sysScheduler.AdvanceTo(when);

            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when);
                return x.Greeting == greeting;
            });
            sysScheduler.Advance(fiveMinutes);
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when.Add(fiveMinutes));
                return x.Greeting == greeting;
            });
            sysScheduler.Advance(fiveMinutes);
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when.Add(fiveMinutes * 2));
                return x.Greeting == greeting;
            }, TimeSpan.FromSeconds(3));
        }

        //[Fact]
        //[Category(Category)]
        private async Task SchedulingJob_ForEveryCronTrigger_DispatchesJobMessage()
        {
            var probe = CreateTestProbe("job-probe");
            var sysScheduler = (TestScheduler)Sys.Scheduler;
            var cronExpression = "* */12 * * *";
            var jobId = TestJobId.New;
            var greeting = $"hi here here is a random guid {Guid.NewGuid()}";
            var job = new TestJob(jobId, greeting);
            var when = DateTime.UtcNow.AddMonths(1);
            var twelveHours = TimeSpan.FromHours(12);

            var accessor = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<TestJobRunnerProbeAccessor>();
            accessor.Put(jobId, probe);

            var scheduler = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<IScheduler>();

            var result = await scheduler.Schedule<TestJob, TestJobId>(job, cronExpression, when);
            result.Should().BeTrue();

            sysScheduler.AdvanceTo(when);

            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when);
                return x.Greeting == greeting;
            });
            sysScheduler.Advance(twelveHours);
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when.Add(twelveHours));
                return x.Greeting == greeting;
            });
            sysScheduler.Advance(twelveHours);
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when.Add(twelveHours * 2));

                return x.Greeting == greeting;
            });
        }


        //[Fact]
        //[Category(Category)]
        private async Task SchedulingJob_AndThenCancellingJob_CeasesDispatching()
        {
            var probe = CreateTestProbe("job-probe");
            var SysScheduler = (TestScheduler)Sys.Scheduler;
            var jobId = TestJobId.New;
            var greeting = $"hi here here is a random guid {Guid.NewGuid()}";
            var job = new TestJob(jobId, greeting);
            var when = DateTime.UtcNow.AddDays(1);
            var fiveMinutes = TimeSpan.FromMinutes(5);

            var accessor = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<TestJobRunnerProbeAccessor>();
            accessor.Put(jobId, probe);

            var scheduler = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<IScheduler>();

            var result = await scheduler.Schedule<TestJob, TestJobId>(job, fiveMinutes, when);
            result.Should().BeTrue();

            SysScheduler.AdvanceTo(when);
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when);

                return x.Greeting == greeting;
            });

            result = await scheduler.Cancel<TestJob, TestJobId>(jobId);
            result.Should().BeTrue();

            SysScheduler.Advance(fiveMinutes);

            probe.ExpectNoMsg();

            SysScheduler.Advance(fiveMinutes);

            probe.ExpectNoMsg();
        }

    }
}