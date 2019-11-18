using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Exceptions;
using EventFly.ReadModels;
using EventFly.Sagas;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventFly.Extensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, AggregateName> SagaNames = new ConcurrentDictionary<Type, AggregateName>();
        public static AggregateName GetSagaName(
            this Type sagaType)
        {
            return SagaNames.GetOrAdd(
                sagaType,
                t =>
                {
                    if (!typeof(IAggregateRoot).GetTypeInfo().IsAssignableFrom(sagaType))
                    {
                        throw new ArgumentException($"Type '{sagaType.PrettyPrint()}' is not a saga.");
                    }

                    return new AggregateName(
                        t.GetTypeInfo().GetCustomAttributes<SagaNameAttribute>().SingleOrDefault()?.Name ??
                        t.Name);
                });
        }

        internal static IReadOnlyList<Tuple<Type, Type>> GetReadModelSubscribersTypes(this Type type)
        {
            var interfaces = type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .ToList();
            var types = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAmReadModelFor<,>))
                .Select(i => new Tuple<Type, Type>(i.GetGenericArguments()[0], i.GetGenericArguments()[1]))
                .ToList();

            return types;
        }

        internal static IReadOnlyDictionary<Type, Action<TReadModel, IDomainEvent>> GetReadModelEventApplyMethods<TReadModel>(this Type type)
            where TReadModel : IReadModel
        {
            var aggregateEventType = typeof(IDomainEvent);

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
                    mi => ReflectionHelper.CompileMethodInvocation<Action<TReadModel, IDomainEvent>>(type, "Apply", mi.GetParameters()[0].ParameterType));
        }

        internal static IReadOnlyList<Type> GetSagaEventSubscriptionTypes(this Type type)
        {
            var interfaces = type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .ToList();

            var handleEventTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISagaHandles<,>))
                .Select(t => typeof(IDomainEvent<,>).MakeGenericType(t.GetGenericArguments()[0],
                    t.GetGenericArguments()[1]))
                .ToList();

            var startedByEventTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISagaIsStartedBy<,>))
                .Select(t => typeof(IDomainEvent<,>).MakeGenericType(t.GetGenericArguments()[0],
                    t.GetGenericArguments()[1]))
                .ToList();

            startedByEventTypes.AddRange(handleEventTypes);

            return startedByEventTypes;
        }
    }
}
