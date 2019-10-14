using System;
using System.ComponentModel;
using System.Reflection;
using GraphQL.Types;

namespace Akkatecture.Web.GraphQL
{
    internal sealed class ObjectGraphTypeFromModel : ObjectGraphType<object>
    {

        public ObjectGraphTypeFromModel(Type modelType, IGraphQueryHandler graphQueryHandler) 
        {
            var modelType1 = modelType;
            IsTypeOf = type => type.GetType().IsAssignableFrom(modelType1);

            Name = !modelType1.Name.EndsWith("Model") ? modelType1.Name : modelType1.Name.Substring(0, modelType1.Name.Length - "Model".Length);
            Description = modelType1.GetCustomAttribute<DescriptionAttribute>()?.Description;

            var fields = QueryParametersHelper.GetFields(modelType1,  graphQueryHandler);
            foreach (var field in fields)
            {
                AddField(field);
            }
        }
    }
}