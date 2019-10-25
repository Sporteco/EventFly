// Decompiled with JetBrains decompiler
// Type: EventFly.Definitions.QueryDefinition
// Assembly: EventFly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 61DF059E-E5F5-4992-B320-644C3E4F5C82
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.dll

using System;

namespace EventFly.Definitions
{
    public class QueryDefinition : IQueryDefinition
    {
        public string Name { get; }

        public Type Type { get; }
        public Type QueryResultType { get; }

        public IQueryManagerDefinition ManagerDefinition { get; }

        public QueryDefinition(Type queryType, Type modelType, IQueryManagerDefinition managerDefinition)
        {
            Type = queryType;
            QueryResultType = modelType;
            Name = GetQueryName(queryType.Name);
            ManagerDefinition = managerDefinition;
        }
        private string GetQueryName(string name) 
            => name.EndsWith("query", StringComparison.InvariantCultureIgnoreCase) ? name.Substring(0, name.Length - "query".Length) : name;

        public override string ToString()
        {
            return Name;
        }
    }
}
