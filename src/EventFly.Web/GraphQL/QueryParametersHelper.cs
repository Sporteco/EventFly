using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using EventFly.Queries;
using GraphQL.Types;

namespace EventFly.GraphQL
{
    internal static class QueryParametersHelper
    {
        public static IGraphType GetQueryItemType(IGraphQueryHandler handler, Type modelType, bool isInput)
        {
            if (!isInput)
            {
                lock (OutputDeclaredTypes)
                {
                    if (OutputDeclaredTypes.ContainsKey(modelType)) return OutputDeclaredTypes[modelType];
                    var result = GetGraphTypeEx(modelType, handler,false);
                    OutputDeclaredTypes.Add(modelType, result);
                    return result;
                }
            }

            lock (InputDeclaredTypes)
            {
                if (InputDeclaredTypes.ContainsKey(modelType)) return InputDeclaredTypes[modelType];
                var result = GetGraphTypeEx(modelType, handler,true);
                InputDeclaredTypes.Add(modelType, result);
                return result;
            }
        }

        private static readonly Dictionary<Type,IGraphType> InputDeclaredTypes = new Dictionary<Type, IGraphType>();
        private static readonly Dictionary<Type,IGraphType> OutputDeclaredTypes = new Dictionary<Type, IGraphType>();


        public static QueryArguments GetArguments(Type parametersType, IGraphQueryHandler graphQuery, bool isInput)
        {
           var qas = new List<QueryArgument>();
            foreach (var prop in parametersType.GetProperties().Where(i=>i.CanWrite))
            {
                var type = GetGraphType(prop.PropertyType,isInput);

                var allowNulls = prop.GetCustomAttribute<AllowNullAttribute>() != null;

                var description = prop.GetCustomAttribute<DescriptionAttribute>()?.Description;

                if (type == null)
                {
                    var gType = GetQueryItemType(graphQuery, prop.PropertyType, true);
                    if (!allowNulls && isInput)
                        gType = new NonNullGraphType(gType);
                    
                    qas.Add(new QueryArgument(gType) {Name = prop.Name, Description = description});
                }
                else
                {
                    if (!allowNulls && isInput)
                    {
                        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(NonNullGraphType<>))
                            type = typeof(NonNullGraphType<>).MakeGenericType(type);
                    }
                    else
                    {
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NonNullGraphType<>))
                            type = type.GetGenericArguments()[0];
                        
                    }

                    qas.Add(new QueryArgument(type) {Name = prop.Name, Description = description});
                }

            }
            return new QueryArguments(qas);

        }

        static readonly Dictionary<Type, Type> MapTypes = new Dictionary<Type, Type>
        {
            {typeof(Guid), typeof(NonNullGraphType<IdGraphType>)},
            {typeof(string), typeof(NonNullGraphType<StringGraphType>)},
            {typeof(bool), typeof(NonNullGraphType<BooleanGraphType>)},
            {typeof(decimal), typeof(NonNullGraphType<DecimalGraphType>)},
            {typeof(float), typeof(NonNullGraphType<FloatGraphType>)},
            {typeof(TimeSpan), typeof(NonNullGraphType<TimeSpanSecondsGraphType>)},
            {typeof(DateTime), typeof(NonNullGraphType<DateTimeGraphType>)},
            {typeof(DateTimeOffset), typeof(NonNullGraphType<DateTimeOffsetGraphType>)},
            {typeof(double), typeof(NonNullGraphType<FloatGraphType>)},
            {typeof(long), typeof(NonNullGraphType<IntGraphType>)},
            {typeof(int), typeof(NonNullGraphType<IntGraphType>)},
            {typeof(Guid?), typeof(IdGraphType)},
            {typeof(bool?), typeof(BooleanGraphType)},
            {typeof(decimal?), typeof(DecimalGraphType)},
            {typeof(float?), typeof(FloatGraphType)},
            {typeof(TimeSpan?), typeof(TimeSpanSecondsGraphType)},
            {typeof(DateTime?), typeof(DateTimeGraphType)},
            {typeof(DateTimeOffset?), typeof(DateTimeOffsetGraphType)},
            {typeof(double?), typeof(FloatGraphType)},
            {typeof(long?), typeof(IntGraphType)},
            {typeof(int?), typeof(IntGraphType)},

        };

        private static Type GetGraphType(Type propType, bool isInput)
        {

            if (propType.IsGenericType && propType.GetGenericArguments().Length == 1 && typeof(IEnumerable).IsAssignableFrom(propType))
            {
                var innerType = GetGraphType(propType.GetGenericArguments().First(), isInput);
                if (innerType != null)
                    return typeof(ListGraphType<>).MakeGenericType(innerType);
                return null;
            }
            if (propType.IsArray)
            {
                var innerType = GetGraphType(propType.GetElementType(),isInput);
                if (innerType != null)
                    return typeof(ListGraphType<>).MakeGenericType(innerType);
                return null;
            }

            if (MapTypes.ContainsKey(propType))
            {
                if (isInput)
                    return MapTypes[propType];
                var type = MapTypes[propType];
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NonNullGraphType<>))
                    type = type.GetGenericArguments()[0];
                return type;
            }

            if (propType.IsEnum)
            {
                return typeof(EnumerationGraphType<>).MakeGenericType(propType);
            }

            if (propType.IsPrimitive)
                return typeof(StringGraphType);
            return null;
        }

        public static IEnumerable<FieldType> GetFields(Type modelType, IGraphQueryHandler graphQuery, bool isInput)
        {
            var qas = new List<FieldType>();
            foreach (var prop in modelType.GetProperties())
            {
                var description = prop.GetCustomAttribute<DescriptionAttribute>()?.Description;

                var allowNulls = prop.GetCustomAttribute<AllowNullAttribute>() != null;

                var type = GetGraphType(prop.PropertyType,isInput);

                if (type == null)
                {
                    var gType = GetQueryItemType(graphQuery, prop.PropertyType, isInput);

                    if (!allowNulls && isInput) gType = new NonNullGraphType(gType);
                    
                    qas.Add(new FieldType { ResolvedType = gType, Name = prop.Name, Description = description});
                }
                else
                {
                    if (!allowNulls && isInput)
                    {
                        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(NonNullGraphType<>))
                            type = typeof(NonNullGraphType<>).MakeGenericType(type);
                    }
                    else
                    {
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NonNullGraphType<>))
                            type = type.GetGenericArguments()[0];
                        
                    }

                    qas.Add(new FieldType {Type = type, Name = prop.Name, Description = description});
                }
            }
            return qas;
        }


        private static IGraphType GetGraphTypeEx(Type propType, IGraphQueryHandler graphQuery, bool isInput)
        {
            if (propType.IsGenericType && propType.GetGenericArguments().Length == 1 && typeof(IEnumerable).IsAssignableFrom(propType))
            {
                var innerType = propType.GetGenericArguments().First();
                if (innerType != null)
                {
                    return new ListGraphType(graphQuery.GetQueryItemType(innerType, isInput));
                }

                return null;
            }

            return  isInput ? new InputObjectGraphTypeFromModel(propType, graphQuery) : 
                (IGraphType)new ObjectGraphTypeFromModel(propType, graphQuery, false);
        }
    }
}
