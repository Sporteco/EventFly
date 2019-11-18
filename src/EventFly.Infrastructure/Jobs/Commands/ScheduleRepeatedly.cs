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

namespace EventFly.Jobs.Commands
{

    public sealed class ScheduleRepeatedly<TJob, TIdentity> : Schedule<TJob, TIdentity>
        where TJob : IJob<TIdentity>
        where TIdentity : IJobId
    {
        public TimeSpan Interval { get; }

        public ScheduleRepeatedly(
            TJob job,
            TimeSpan interval,
            DateTime triggerDate,
            Object ack = null,
            Object nack = null)
            : base(job, triggerDate, ack, nack)
        {
            if (interval == default) throw new ArgumentException(nameof(interval));

            Interval = interval;
        }

        public override Schedule<TJob, TIdentity> WithNextTriggerDate(DateTime utcDate) => new ScheduleRepeatedly<TJob, TIdentity>(Job, Interval, TriggerDate + Interval);

        public override Schedule<TJob, TIdentity> WithAck(Object ack)
        {
            return new ScheduleRepeatedly<TJob, TIdentity>(Job, Interval, TriggerDate, ack, Nack);
        }

        public override Schedule<TJob, TIdentity> WithNack(Object nack)
        {
            return new ScheduleRepeatedly<TJob, TIdentity>(Job, Interval, TriggerDate, Ack, nack);
        }
    }
}