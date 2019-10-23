﻿using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Sagas;

namespace EventFly.DomainService
{
    public interface IDomainServiceIsStartedByAsync<in TIdentity, in TAggregateEvent> : ISagaHandlesAsync<TIdentity, TAggregateEvent>
        where TAggregateEvent : class, IAggregateEvent<TIdentity>
        where TIdentity : IIdentity
    {
    }
}