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

using Cronos;
using System;

namespace EventFly.Jobs.Commands
{

    public sealed class ScheduleCron<TJob, TIdentity> : Schedule<TJob, TIdentity>
        where TJob : IJob<TIdentity>
        where TIdentity : IJobId
    {
        public String CronExpression { get; }
        private readonly CronExpression _expression;

        public ScheduleCron(
            TJob job,
            String cronExpression,
            DateTime triggerDate,
            Object ack = null,
            Object nack = null)
            : base(job, triggerDate, ack, nack)
        {
            if (String.IsNullOrWhiteSpace(cronExpression)) throw new ArgumentNullException(nameof(cronExpression));

            CronExpression = cronExpression;
            _expression = Cronos.CronExpression.Parse(cronExpression);
        }

        public override Schedule<TJob, TIdentity> WithNextTriggerDate(DateTime utcDate)
        {
            var next = _expression.GetNextOccurrence(utcDate);
            if (next.HasValue)
                return new ScheduleCron<TJob, TIdentity>(Job, CronExpression, next.Value);

            return null;
        }

        public override Schedule<TJob, TIdentity> WithAck(Object ack)
        {
            return new ScheduleCron<TJob, TIdentity>(Job, CronExpression, TriggerDate, ack, Nack);
        }

        public override Schedule<TJob, TIdentity> WithNack(Object nack)
        {
            return new ScheduleCron<TJob, TIdentity>(Job, CronExpression, TriggerDate, Ack, nack);
        }
    }
}