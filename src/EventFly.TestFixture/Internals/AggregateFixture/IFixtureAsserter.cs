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
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.Core;

namespace EventFly.TestFixture.Internals.AggregateFixture
{
    public interface IFixtureAsserter<TAggregate, TIdentity>
        where TAggregate : ActorBase, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        IFixtureAsserter<TAggregate, TIdentity> AndWhen(params ICommand[] commands);

        IFixtureAsserter<TAggregate, TIdentity> ThenExpect<TAggregateEvent>(Predicate<TAggregateEvent> aggregateEventPredicate = null, TimeSpan? timeout = null)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>;

        IFixtureAsserter<TAggregate, TIdentity> ThenExpectReply<TReply>(Predicate<TReply> aggregateReply = null, TimeSpan? timeout = null);

        IFixtureAsserter<TAggregate, TIdentity> ThenExpectDomainEvent<TAggregateEvent>(Predicate<IDomainEvent<TIdentity, TAggregateEvent>> domainEventPredicate = null, TimeSpan? timeout = null)
            where TAggregateEvent : class, IAggregateEvent<TIdentity>;
    }
}