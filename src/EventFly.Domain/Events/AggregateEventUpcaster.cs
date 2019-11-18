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

using Akka.Persistence.Journal;
using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Exceptions;
using EventFly.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EventFly.Events
{
    public abstract class AggregateEventUpcaster<TAggregate, TIdentity> : AggregateEventUpcaster<TAggregate, TIdentity, IEventUpcaster<TAggregate, TIdentity>>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {

    }

    public abstract class AggregateEventUpcaster<TAggregate, TIdentity, TEventUpcaster> : IReadEventAdapter, IEventUpcaster<TAggregate, TIdentity>
        where TEventUpcaster : class, IEventUpcaster<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        // ReSharper disable once StaticMemberInGenericType
        private static ConcurrentDictionary<Type, Boolean> _decisionCache = new ConcurrentDictionary<Type, Boolean>();
        private readonly IReadOnlyDictionary<Type, Func<TEventUpcaster, IAggregateEvent, IAggregateEvent>> _upcastFunctions;


        protected AggregateEventUpcaster()
        {
            var upcastTypes = GetType().GetAggregateEventUpcastTypes();
            var dictionary = upcastTypes.ToDictionary(x => x, x => true);
            _decisionCache = new ConcurrentDictionary<Type, Boolean>(dictionary);

            var me = this as TEventUpcaster;
            if (me == null)
            {
                throw new InvalidOperationException(
                    $"Event applier of type '{GetType().PrettyPrint()}' has a wrong generic argument '{typeof(TEventUpcaster).PrettyPrint()}'");
            }
            _upcastFunctions = GetType().GetAggregateEventUpcastMethods<TAggregate, TIdentity, TEventUpcaster>();
        }

        private Boolean ShouldUpcast(Object potentialUpcast)
        {
            var type = potentialUpcast.GetType();

            if (potentialUpcast is ICommittedEvent<TIdentity>)
            {
                var eventType = type.GenericTypeArguments[1];

                if (_decisionCache.ContainsKey(eventType))
                {

                    return true;
                }
            }

            return false;
        }

        public IEventSequence FromJournal(Object evt, String manifest)
        {
            if (ShouldUpcast(evt))
            {
                //dynamic dispatch here to get AggregateEvent
                var committedEvent = evt as dynamic;

                var upcastedEvent = Upcast(committedEvent.AggregateEvent);

                var genericType = typeof(CommittedEvent<,>)
                    .MakeGenericType(typeof(TIdentity), upcastedEvent.GetType());

                var upcastedCommittedEvent = Activator.CreateInstance(
                    genericType,
                    committedEvent.AggregateIdentity,
                    upcastedEvent,
                    committedEvent.EventMetadata,
                    committedEvent.Timestamp,
                    committedEvent.AggregateSequenceNumber);

                return EventSequence.Single(upcastedCommittedEvent);
            }

            return EventSequence.Single(evt);
        }



        public IAggregateEvent<TIdentity> Upcast(
            IAggregateEvent<TIdentity> aggregateEvent)
        {
            var aggregateEventType = aggregateEvent.GetType();
            Func<TEventUpcaster, IAggregateEvent, IAggregateEvent> upcaster;

            if (!_upcastFunctions.TryGetValue(aggregateEventType, out upcaster))
            {
                throw new ArgumentException(nameof(aggregateEventType));
            }

            var evt = upcaster((TEventUpcaster)(Object)this, aggregateEvent) as IAggregateEvent<TIdentity>;

            return evt;
        }
    }
}