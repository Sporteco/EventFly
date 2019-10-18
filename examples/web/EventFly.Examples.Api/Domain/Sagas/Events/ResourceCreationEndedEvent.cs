﻿// The MIT License (MIT)
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
using EventFly.Aggregates;
using EventFly.Examples.Api.Domain.Aggregates.Resource;

namespace EventFly.Examples.Api.Domain.Sagas.Events
{
    public class ResourceCreationEndedEvent : AggregateEvent<ResourceCreationSagaId>
    {
        public ResourceId ResourceId { get; }
        public int Progress { get; }
        public int Elapsed { get; }
        public DateTime EndedAt { get; }

        public ResourceCreationEndedEvent(
            ResourceId resourceId,
            int progress,
            int elapsed,
            DateTime endedAt)
        {
            ResourceId = resourceId;
            Progress = progress;
            Elapsed = elapsed;
            EndedAt = endedAt;
        }
    }
}