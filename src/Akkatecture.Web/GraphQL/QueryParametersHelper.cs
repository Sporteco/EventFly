using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using GraphQL.Types;

namespace Akkatecture.Web.GraphQL
{
    internal static class QueryParametersHelper
    {
        public static QueryArguments GetArguments(Type parametersType, IGraphQueryHandler graphQuery)
        {
            var qas = new List<QueryArgument>();
            foreach (var prop in parametersType.GetProperties().Where(i=>i.CanWrite))
            {
                var type = GetGraphType(prop.PropertyType);

                var description = prop.GetCustomAttribute<DescriptionAttribute>()?.Description;
                qas.Add(type != null
                    ? new QueryArgument(type) {Name = prop.Name, Description = description}
                    : new QueryArgument(GetGraphTypeEx(prop.PropertyType, graphQuery)) {Name = prop.Name, Description = description});
            }
            return new QueryArguments(qas);
        }

        static readonly Dictionary<Type, Type> MapTypes = new Dictionary<Type, Type>
        {
            {typeof(Guid), typeof(IdGraphType)},
            {typeof(string), typeof(StringGraphType)},
            {typeof(bool), typeof(BooleanGraphType)},
            {typeof(decimal), typeof(DecimalGraphType)},
            {typeof(float), typeof(FloatGraphType)},
            {typeof(TimeSpan), typeof(TimeSpanSecondsGraphType)},
            {typeof(DateTime), typeof(DateGraphType)},
            {typeof(DateTimeOffset), typeof(DateTimeOffsetGraphType)},
            {typeof(double), typeof(FloatGraphType)},
            {typeof(long), typeof(IntGraphType)},
            {typeof(int), typeof(IntGraphType)},
            {typeof(Guid?), typeof(IdGraphType)},
            {typeof(bool?), typeof(BooleanGraphType)},
            {typeof(decimal?), typeof(DecimalGraphType)},
            {typeof(float?), typeof(FloatGraphType)},
            {typeof(TimeSpan?), typeof(TimeSpanSecondsGraphType)},
            {typeof(DateTime?), typeof(DateGraphType)},
            {typeof(DateTimeOffset?), typeof(DateTimeOffsetGraphType)},
            {typeof(double?), typeof(FloatGraphType)},
            {typeof(long?), typeof(IntGraphType)},
            {typeof(int?), typeof(IntGraphType)},

        };

        private static Type GetGraphType(Type propType)
        {

            if (propType.IsGenericType && propType.GetGenericArguments().Length == 1 && typeof(IEnumerable).IsAssignableFrom(propType))
            {
                var innerType = GetGraphType(propType.GetGenericArguments().First());
                if (innerType != null)
                    return typeof(ListGraphType<>).MakeGenericType(innerType);
                return null;
            }
            if (MapTypes.ContainsKey(propType))
                return MapTypes[propType];

            if (propType.IsPrimitive)
                return typeof(StringGraphType);
            return null;
        }

        public static IEnumerable<FieldType> GetFields(Type modelType, IGraphQueryHandler graphQuery)
        {
            var qas = new List<FieldType>();
            foreach (var prop in modelType.GetProperties())
            {
                var description = prop.GetCustomAttribute<DescriptionAttribute>()?.Description;
                var type = GetGraphType(prop.PropertyType);
                qas.Add(type != null
                    ? new FieldType {Type = type, Name = prop.Name, Description = description}
                    : new FieldType
                    {
                        ResolvedType = GetGraphTypeEx(prop.PropertyType, graphQuery), Name = prop.Name,
                        Description = description,
                    });
            }
            return qas;
        }


        private static IGraphType GetGraphTypeEx(Type propType, IGraphQueryHandler graphQuery)
        {
            if (propType.IsGenericType && propType.GetGenericArguments().Length == 1 && typeof(IEnumerable).IsAssignableFrom(propType))
            {
                var innerType = propType.GetGenericArguments().First();
                if (innerType != null)
                    return new ListGraphType(graphQuery.GetQueryItemType(innerType));
                return null;
            }

            return graphQuery.GetQueryItemType(propType);
        }
    }
}
