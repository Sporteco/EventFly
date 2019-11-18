using EventFly.Definitions;
using GraphQL.Types;
using System;
using System.ComponentModel;
using System.Reflection;
using Type = System.Type;

namespace EventFly.GraphQL
{
    internal sealed class ObjectGraphTypeFromModel : ObjectGraphType<Object>
    {

        public ObjectGraphTypeFromModel(Type modelType, IGraphQueryHandler graphQueryHandler, Boolean isInput)
        {
            var modelType1 = modelType;
            IsTypeOf = type => type.GetType().IsAssignableFrom(modelType1);

            Name = !modelType1.Name.EndsWith("Model") ? modelType1.Name : modelType1.Name.Substring(0, modelType1.Name.Length - "Model".Length);
            Description = modelType1.GetCustomAttribute<DescriptionAttribute>()?.Description;

            var fields = QueryParametersHelper.GetFields(modelType1, graphQueryHandler, isInput);
            foreach (var field in fields)
            {
                AddField(field);
            }
        }
    }
    internal sealed class ObjectGraphTypeFromDomain : ObjectGraphType<Object>
    {

        public ObjectGraphTypeFromDomain(IContextDefinition domainDefinition, IServiceProvider provider)
        {
            var modelType1 = domainDefinition.GetType();
            IsTypeOf = type => true;

            Name = modelType1.Name;
            Description = modelType1.GetCustomAttribute<DescriptionAttribute>()?.Description;

            foreach (var query in domainDefinition.Queries)
            {
                var gQueryType = typeof(IGraphQueryHandler<,>).MakeGenericType(query.Type, query.QueryResultType);
                var handler = (IGraphQueryHandler)provider.GetService(gQueryType);

                AddField(handler.GetFieldType(false));
            }
        }

    }


    internal sealed class InputObjectGraphTypeFromModel : InputObjectGraphType<Object>
    {

        public InputObjectGraphTypeFromModel(Type modelType, IGraphQueryHandler graphQueryHandler)
        {
            var modelType1 = modelType;
            //IsTypeOf = type => type.GetType().IsAssignableFrom(modelType1);

            Name = "Input" + (!modelType1.Name.EndsWith("Model") ? modelType1.Name : modelType1.Name.Substring(0, modelType1.Name.Length - "Model".Length));
            Description = modelType1.GetCustomAttribute<DescriptionAttribute>()?.Description;

            var fields = QueryParametersHelper.GetFields(modelType1, graphQueryHandler, true);
            foreach (var field in fields)
            {
                AddField(field);
            }
        }
    }

}