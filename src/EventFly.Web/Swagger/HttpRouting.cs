// Decompiled with JetBrains decompiler
// Type: EventFly.Web.Swagger.HttpRouting
// Assembly: EventFly.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.Web.dll

namespace EventFly.Swagger
{
    public static class HttpRouting
    {
        public const System.String AggregateIdRouteParam = "aggregateId";
        public const System.String DomainRouteParam = "domainRouteParam";
        public const System.String AggregateRouteParam = "aggregateRouteParam";
        public const System.String ActionRouteParam = "actionRouteParam";
        public const System.String ActionFormat = "api/{DomainName}/{AggregateName}/{AggregateId}/{ActionName}";
        public const System.String SingletonActionFormat = "api/{DomainName}/{AggregateName}/{ActionName}";

        public class Params
        {
            public readonly System.String AggregateId = "{aggregateId}";

            public static Params ForController { get; } = new Params { DomainName = "{domainRouteParam}", AggregateName = "{aggregateRouteParam}", ActionName = "{actionRouteParam}" };

            public System.String DomainName { get; set; }

            public System.String AggregateName { get; set; }

            public System.String ActionName { get; set; }
        }
    }
}
