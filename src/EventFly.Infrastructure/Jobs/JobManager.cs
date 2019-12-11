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
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Akka.Pattern;
using EventFly.Extensions;
using EventFly.Infrastructure.Extensions;
using EventFly.Jobs;

namespace EventFly.Infrastructure.Jobs
{
    public class JobManager<TJobScheduler, TJobRunner, TJob, TIdentity> : ReceiveActor, IJobManager<TJob, TIdentity>
        where TJobScheduler : JobScheduler<TJobScheduler, TJob, TIdentity>
        where TJobRunner : JobRunner<TJob, TIdentity>
        where TJob : IJob<TIdentity>
        where TIdentity : IJobId
    {
        private static readonly IJobName JobName = typeof(TJob).GetJobName();
        public IJobName Name => JobName;
        protected ILoggingAdapter Log { get; }
        protected IActorRef JobRunner { get; }
        protected IActorRef JobScheduler { get; }
        public JobManager()
        {
            var schedulerProps = Props.Create(typeof(TJobScheduler)).WithDispatcher(Context.Props.Dispatcher);
            var schedulerName = $"{Name}-scheduler";

            var schedulerSupervisorProps =
                BackoffSupervisor.Props(
                    Backoff.OnFailure(
                        schedulerProps,
                        schedulerName,
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(60),
                        0.2,
                        3)).WithDispatcher(Context.Props.Dispatcher);

            JobRunner = SpawnRunner();
            JobScheduler = Context.ActorOf(schedulerSupervisorProps, $"{schedulerName}-supervisor");

            Log = Context.GetLogger();

            Receive<TJob>(Forward);
            Receive<SchedulerMessage<TJob, TIdentity>>(Forward);
        }

        private Boolean Forward(TJob command)
        {
            Log.Info("JobManager for Job of Name={0} is forwarding job command of Type={1} to JobRunner at ActorPath={2}", Name, typeof(TJob).PrettyPrint(), JobRunner.Path);
            JobRunner.Forward(command);
            return true;
        }

        private Boolean Forward(SchedulerMessage<TJob, TIdentity> command)
        {
            Log.Info("JobManager for Job of Name={0} is forwarding job command of Type={1} to JobScheduler at ActorPath={2}", Name, typeof(TJob).PrettyPrint(), JobScheduler.Path);
            JobScheduler.Forward(command);
            return true;
        }

        protected IActorRef FindOrSpawnJobRunner()
        {
            var runner = Context.Child($"{Name}-runner-supervisor");
            if (runner.IsNobody())
            {
                return SpawnRunner();
            }
            return runner;
        }

        //TODO: move to constructor
        private IActorRef SpawnRunner()
        {
            Props runnerProps;
            try
            {
                runnerProps = Context.DI().Props<TJobRunner>().WithDispatcher(Context.Props.Dispatcher);
            }
            catch (Exception ex)
            {
                Context.GetLogger().Error(ex, "No DI available at the moment, falling back to default props creation.");
                runnerProps = Props.Create<TJobRunner>().WithDispatcher(Context.Props.Dispatcher);
            }

            var runnerSupervisorProps =
                BackoffSupervisor.Props(
                    Backoff.OnFailure(
                        runnerProps,
                        $"{Name}-runner",
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(60),
                        0.2,
                        3)).WithDispatcher(Context.Props.Dispatcher);

            var runner = Context.ActorOf(runnerSupervisorProps, $"{Name}-runner-supervisor");
            return runner;
        }
    }
}