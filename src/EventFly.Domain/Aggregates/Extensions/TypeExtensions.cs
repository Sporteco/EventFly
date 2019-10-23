using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventFly.Domain.Aggregates
{
    internal static class TypeExtensions
    {
        internal static IReadOnlyList<Tuple<Type, Type>> GetAggregateExecuteTypes(this Type type)
        {
            var interfaces = type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .ToList();
            var domainEventTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IExecute<,,>))
                .Select(i => new Tuple<Type, Type>(i.GetGenericArguments()[0], i.GetGenericArguments()[1]))
                .ToList();


            return domainEventTypes;
        }
    }
}
