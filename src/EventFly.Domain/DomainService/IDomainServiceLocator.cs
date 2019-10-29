using EventFly.Aggregates;
using EventFly.Core;
using System;

namespace EventFly.Domain
{
    public interface IDomainServiceLocator<out TIdentity>
        where TIdentity : IIdentity
    {
        TIdentity LocateDomainService(IDomainEvent domainEvent);
    }

    internal class EmptyIdentity : Identity<EmptyIdentity> { public EmptyIdentity(String value) : base(value) { } }
}