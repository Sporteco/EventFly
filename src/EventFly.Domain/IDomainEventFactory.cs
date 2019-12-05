using EventFly.Aggregates;
using EventFly.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace EventFly
{
    public interface IDomainEventFactory
    {
        IDomainEvent Create(IAggregateEvent aggregateEvent, IEventMetadata metadata);
    }

    internal sealed class DomainEventFactory : IDomainEventFactory
    {
        private static readonly ConcurrentDictionary<Type, Type> _aggregateEventToDomainEventTypeMap = new ConcurrentDictionary<Type, Type>();
        private static readonly ConcurrentDictionary<Type, Type> _domainEventToIdentityTypeMap = new ConcurrentDictionary<Type, Type>();

        public IDomainEvent Create(IAggregateEvent aggregateEvent, IEventMetadata metadata)
        {
            var domainEventType = _aggregateEventToDomainEventTypeMap.GetOrAdd(aggregateEvent.GetType(), GetDomainEventType);
            var identityType = _domainEventToIdentityTypeMap.GetOrAdd(domainEventType, GetIdentityType);
            var identity = Activator.CreateInstance(identityType, metadata.AggregateId);

            var domainEvent = (IDomainEvent)Activator.CreateInstance(
                domainEventType,
                identity,
                aggregateEvent,
                metadata,
                metadata.Timestamp,
                metadata.AggregateSequenceNumber);

            return domainEvent;
        }

        private static Type GetIdentityType(Type domainEventType)
        {
            var domainEventInterfaceType = domainEventType
                .GetTypeInfo()
                .GetInterfaces()
                .SingleOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEvent<>));

            if (domainEventInterfaceType == null)
                throw new ArgumentException($"Type '{domainEventType.PrettyPrint()}' is not a '{typeof(IDomainEvent<>).PrettyPrint()}'");

            var genericArguments = domainEventInterfaceType.GetTypeInfo().GetGenericArguments();
            return genericArguments[0];
        }

        private static Type GetDomainEventType(Type aggregateEventType)
        {
            var aggregateEventInterfaceType = aggregateEventType
                .GetTypeInfo()
                .GetInterfaces()
                .SingleOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IAggregateEvent<>));

            if (aggregateEventInterfaceType == null)
                throw new ArgumentException($"Type '{aggregateEventType.PrettyPrint()}' is not a '{typeof(IAggregateEvent<>).PrettyPrint()}'");

            var genericArguments = aggregateEventInterfaceType.GetTypeInfo().GetGenericArguments();
            return typeof(DomainEvent<,>).MakeGenericType(genericArguments[0], aggregateEventType);
        }
    }
}
