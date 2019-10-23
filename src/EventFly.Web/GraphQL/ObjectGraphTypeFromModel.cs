using System;
using System.ComponentModel;
using System.Reflection;
using EventFly.Definitions;
using GraphQL.Types;
using Type = System.Type;

namespace EventFly.Web.GraphQL
{
    internal sealed class ObjectGraphTypeFromModel : ObjectGraphType<object>
    {

        public ObjectGraphTypeFromModel(Type modelType, IGraphQueryHandler graphQueryHandler) 
        {
            var modelType1 = modelType;
            IsTypeOf = type => type.GetType().IsAssignableFrom(modelType1);

            Name = !modelType1.Name.EndsWith("Model") ? modelType1.Name : modelType1.Name.Substring(0, modelType1.Name.Length - "Model".Length);
            Description = modelType1.GetCustomAttribute<DescriptionAttribute>()?.Description;

            var fields = QueryParametersHelper.GetFields(modelType1,  graphQueryHandler, false);
            foreach (var field in fields)
            {
                AddField(field);
            }
        }
    }
    internal sealed class ObjectGraphTypeFromDomain : ObjectGraphType<object>
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
                var handler = (IGraphQueryHandler) provider.GetService(gQueryType);

                AddField(handler.GetFieldType(false));
            }
        }

    }


    internal sealed class InputObjectGraphTypeFromModel : InputObjectGraphType<object>
    {

        public InputObjectGraphTypeFromModel(Type modelType, IGraphQueryHandler graphQueryHandler) 
        {
            var modelType1 = modelType;
            //IsTypeOf = type => type.GetType().IsAssignableFrom(modelType1);

            Name = !modelType1.Name.EndsWith("Model") ? modelType1.Name : modelType1.Name.Substring(0, modelType1.Name.Length - "Model".Length);
            Description = modelType1.GetCustomAttribute<DescriptionAttribute>()?.Description;

            var fields = QueryParametersHelper.GetFields(modelType1,  graphQueryHandler, true);
            foreach (var field in fields)
            {
                AddField(field);
            }
        }
    }
    
}