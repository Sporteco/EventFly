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
using EventFly.Exceptions;
using EventFly.Jobs.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventFly.Jobs
{
    internal sealed class JobToJobMannagerScheduler : IScheduler
    {
        private readonly IDefinitionToManagerRegistry _definitionToManagerRegistry;

        public JobToJobMannagerScheduler(IDefinitionToManagerRegistry definitionToManagerRegistry)
        {
            _definitionToManagerRegistry = definitionToManagerRegistry;
        }

        public Task<Boolean> Schedule<TJob, TJobId>(TJob job, DateTime triggerDate) where TJob : IJob<TJobId> where TJobId : IJobId
        {
            var manager = GetJobManager(job.JobId.GetType());

            return manager.Ask<Boolean>(
                new Schedule<TJob, TJobId>(job, triggerDate)
                    .WithAck(true)
                    .WithNack(false)
            );
        }

        public Task<Boolean> Schedule<TJob, TJobId>(TJob job, TimeSpan interval, DateTime triggerDate) where TJob : IJob<TJobId> where TJobId : IJobId
        {
            var manager = GetJobManager(job.JobId.GetType());

            return manager.Ask<Boolean>(
                new ScheduleRepeatedly<TJob, TJobId>(job, interval, triggerDate)
                    .WithAck(true)
                    .WithNack(false)
            );
        }

        public Task<Boolean> Schedule<TJob, TJobId>(TJob job, String cronExpression, DateTime triggerDate) where TJob : IJob<TJobId> where TJobId : IJobId
        {
            var manager = GetJobManager(job.JobId.GetType());

            return manager.Ask<Boolean>(
                new ScheduleCron<TJob, TJobId>(job, cronExpression, triggerDate)
                    .WithAck(true)
                    .WithNack(false)
            );
        }

        public Task<Boolean> Cancel<TJob, TJobId>(TJobId jobId) where TJob : IJob<TJobId> where TJobId : IJobId
        {
            var manager = GetJobManager(jobId.GetType());

            return manager.Ask<Boolean>(
                new Cancel<TJob, TJobId>(jobId)
                    .WithAck(true)
                    .WithNack(false)
            );
        }

        private IActorRef GetJobManager(Type type)
        {
            var manager = _definitionToManagerRegistry.DefinitionToJobManager.FirstOrDefault(i =>
            {
                var (k, _) = (i.Key, i.Value);
                if (!(k.JobType == type))
                    return k.IdentityType == type;
                return true;
            }).Value;

            if (manager == null)
                throw new InvalidOperationException("Job " + type.PrettyPrint() + " not registered");
            return manager;
        }
    }
}