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

using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using EventFly.Infrastructure.Jobs;
using EventFly.Jobs;

namespace EventFly.Tests.Data.Application.Jobs
{
    public class TestJobRunner : JobRunner<TestJob, TestJobId>, IRun<TestJob, TestJobId>
    {
        public TestJobRunner(TestJobRunnerProbeAccessor accessor)
        {
            _accessor = accessor;
        }

        public Boolean Run(TestJob job)
        {
            var time = Context.System.Settings.System.Scheduler.Now.DateTime;
            var probe = _accessor.Get(job.JobId);
            probe.Tell(new TestJobDone(job.Greeting, time));
            Context.GetLogger().Info("JobRunner has processed TestJob.");
            return true;
        }

        private readonly TestJobRunnerProbeAccessor _accessor;
    }

    public class AsyncTestJobRunner : JobRunner<AsyncTestJob, AsyncTestJobId>, IRunAsync<AsyncTestJob, AsyncTestJobId>
    {
        public AsyncTestJobRunner(TestJobRunnerProbeAccessor accessor)
        {
            _accessor = accessor;
        }

        public async Task<Boolean> RunAsync(AsyncTestJob job)
        {
            var time = Context.System.Settings.System.Scheduler.Now.DateTime;
            var probe = _accessor.Get(job.JobId);
            probe.Tell(new TestJobDone(job.Greeting, time));
            Context.GetLogger().Info("AsyncJobRunner has processed TestJob.");
            return await Task.FromResult(true);
        }

        private readonly TestJobRunnerProbeAccessor _accessor;
    }

    public class TestJobDone
    {
        public String Greeting { get; }
        public DateTime At { get; }

        public TestJobDone(String greeting, DateTime at)
        {
            Greeting = greeting;
            At = at;
        }
    }
}