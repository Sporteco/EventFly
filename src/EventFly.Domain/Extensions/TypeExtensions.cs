// The MIT License (MIT)
//
// Copyright (c) 2015-2019 Rasmus Mikkelsen
// Copyright (c) 2015-2019 eBay Software Foundation
// Modified from original source https://github.com/eventflow/EventFlow
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

using EventFly.Aggregates;
using EventFly.Aggregates.Snapshot;
using EventFly.Core;
using EventFly.DomainService;
using EventFly.Events;
using EventFly.Exceptions;
using EventFly.Subscribers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventFly.Extensions
{
    public static class TypeExtensions
    {
        public static AggregateName GetAggregateName(this Type aggregateType)
        {
            return AggregateNames.GetOrAdd(
                aggregateType,
                t =>
                {
                    if (!typeof(IAggregateRoot).GetTypeInfo().IsAssignableFrom(aggregateType) 
                        && !typeof(IIdentity).GetTypeInfo().IsAssignableFrom(aggregateType))
                    {
                        throw new ArgumentException($"Type '{aggregateType.PrettyPrint()}' is not an aggregate root or identity");
                    }

                    return new AggregateName(
                        t.GetTypeInfo().GetCustomAttributes<AggregateNameAttribute>().SingleOrDefault()?.Name ??
                        t.Name);
                });
        }

        internal static IReadOnlyDictionary<Type, Action<T, IAggregateEvent>> GetAggregateEventApplyMethods<TIdentity, T>(this Type type)
            where TIdentity : IIdentity
        {
            var aggregateEventType = typeof(IAggregateEvent<TIdentity>);

            return type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Apply") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1 &&
                        aggregateEventType.GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType);
                })
                .ToDictionary(
                    mi => mi.GetParameters()[0].ParameterType,
                    mi => ReflectionHelper.CompileMethodInvocation<Action<T, IAggregateEvent>>(type, "Apply", mi.GetParameters()[0].ParameterType));
        }

        internal static IReadOnlyDictionary<Type, Action<T, IAggregateSnapshot>> GetAggregateSnapshotHydrateMethods<TAggregate, TIdentity, T>(this Type type)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateSnapshot = typeof(IAggregateSnapshot<TAggregate, TIdentity>);

            return type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Hydrate") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1 &&
                        aggregateSnapshot.GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType);
                })
                .ToDictionary(
                    mi => mi.GetParameters()[0].ParameterType,
                    mi => ReflectionHelper.CompileMethodInvocation<Action<T, IAggregateSnapshot>>(type, "Hydrate", mi.GetParameters()[0].ParameterType));
        }

        internal static IReadOnlyDictionary<Type, Action<TAggregateState, IAggregateEvent>> GetAggregateSagaEventApplyMethods<TAggregate, TIdentity, TAggregateState>(this Type type)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateEventType = typeof(IAggregateEvent<TIdentity>);


            return type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Apply") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1 &&
                        aggregateEventType.GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType);
                })
                .ToDictionary(
                    mi => mi.GetParameters()[0].ParameterType,
                    mi => ReflectionHelper.CompileMethodInvocation<Action<TAggregateState, IAggregateEvent>>(type, "Apply", mi.GetParameters()[0].ParameterType));
        }

        internal static IReadOnlyList<Type> GetAsyncDomainEventSubscriberSubscriptionTypes(this Type type)
        {
            var interfaces = type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .ToList();
            var domainEventTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscribeToAsync<,>))
                .Select(i => typeof(IDomainEvent<,>).MakeGenericType(i.GetGenericArguments()[0], i.GetGenericArguments()[1]))
                .ToList();

            return domainEventTypes;
        }

        internal static IReadOnlyList<Tuple<Type, Type>> GetAggregateExecuteTypes(this Type type)
        {
            var interfaces = type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .ToList();
            var domainEventTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IExecute<,>))
                .Select(i => new Tuple<Type, Type>(i.GetGenericArguments()[0], i.GetGenericArguments()[1]))
                .ToList();

            return domainEventTypes;
        }

        internal static IReadOnlyList<Type> GetDomainEventSubscriberSubscriptionTypes(this Type type)
        {
            var interfaces = type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .ToList();
            var domainEventTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscribeTo<,>))
                .Select(i => typeof(IDomainEvent<,>).MakeGenericType(i.GetGenericArguments()[0], i.GetGenericArguments()[1]))
                .ToList();

            return domainEventTypes;
        }

        internal static AggregateName GetCommittedEventAggregateRootName(this Type type)
        {
            return AggregateNameCache.GetOrAdd(
                type,
                t =>
                {
                    var interfaces = type
                        .GetTypeInfo()
                        .GetInterfaces()
                        .Select(i => i.GetTypeInfo())
                        .ToList();

                    var aggregateType = interfaces
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommittedEvent<,>))
                        .Select(i => i.GetGenericArguments()[0]).SingleOrDefault();

                    if (aggregateType != null) return aggregateType.GetAggregateName();
                    throw new ArgumentException(nameof(type));
                });
        }

        internal static Type GetCommittedEventAggregateEventType(this Type type)
        {
            return AggregateEventTypeCache.GetOrAdd(
                type,
                t =>
                {
                    var interfaces = type
                        .GetTypeInfo()
                        .GetInterfaces()
                        .Select(i => i.GetTypeInfo())
                        .ToList();

                    var aggregateEvent = interfaces
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommittedEvent<,>))
                        .Select(i => i.GetGenericArguments()[1]).SingleOrDefault();

                    if (aggregateEvent != null) return aggregateEvent;
                    throw new ArgumentException(nameof(type));
                });
        }

        internal static IReadOnlyList<Type> GetDomainServiceEventSubscriptionTypes(this Type type)
        {
            var interfaces = type.GetTypeInfo().GetInterfaces().ToList();
            var handleEventTypes = interfaces
                .Where(i => i.IsGenericType)
                .Where(i => i.GetGenericTypeDefinition() == typeof(IDomainServiceHandles<,>))
                .Select(t => typeof(IDomainEvent<,>).MakeGenericType(t.GetGenericArguments()[0], t.GetGenericArguments()[1]))
                .ToList();

            return handleEventTypes;
        }

        internal static IReadOnlyList<Type> GetAsyncDomainServiceEventSubscriptionTypes(this Type type)
        {
            var interfaces = type.GetTypeInfo().GetInterfaces().ToList();
            var handleEventTypes = interfaces
                .Where(i => i.IsGenericType)
                .Where(i => i.GetGenericTypeDefinition() == typeof(IDomainServiceHandlesAsync<,>))
                .Select(t => typeof(IDomainEvent<,>).MakeGenericType(t.GetGenericArguments()[0], t.GetGenericArguments()[1]))
                .ToList();

            return handleEventTypes;
        }

        internal static IReadOnlyDictionary<Type, Func<T, IAggregateEvent, IAggregateEvent>> GetAggregateEventUpcastMethods<TAggregate, TIdentity, T>(this Type type)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateEventType = typeof(IAggregateEvent<TIdentity>);

            return type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Upcast") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1 &&
                        aggregateEventType.GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType);
                })
                .ToDictionary(
                    //problem might be here
                    mi => mi.GetParameters()[0].ParameterType,
                    mi => ReflectionHelper.CompileMethodInvocation<Func<T, IAggregateEvent, IAggregateEvent>>(type, "Upcast", mi.GetParameters()[0].ParameterType));
        }

        internal static IReadOnlyList<Type> GetAggregateEventUpcastTypes(this Type type)
        {
            var interfaces = type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .ToList();

            var upcastableEventTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUpcast<,>))
                .Select(i => i.GetGenericArguments()[0])
                .ToList();

            return upcastableEventTypes;
        }

        internal static Type GetBaseType(this Type type, String name)
        {
            var currentType = type;
            while (currentType != null)
            {
                if (currentType.Name.Contains(name)) return currentType;
                currentType = currentType.BaseType;
            }
            return type;
        }

        private static readonly ConcurrentDictionary<Type, AggregateName> AggregateNames = new ConcurrentDictionary<Type, AggregateName>();
        private static readonly ConcurrentDictionary<Type, AggregateName> AggregateNameCache = new ConcurrentDictionary<Type, AggregateName>();
        private static readonly ConcurrentDictionary<Type, Type> AggregateEventTypeCache = new ConcurrentDictionary<Type, Type>();
    }
}