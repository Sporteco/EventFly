// Decompiled with JetBrains decompiler
// Type: EventFly.Web.Swagger.HttpRouting
// Assembly: EventFly.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\EventFly.Web.dll

namespace EventFly.Web.Swagger
{
  public static class HttpRouting
  {
    public const string AggregateIdRouteParam = "aggregateId";
    public const string DomainRouteParam = "domainRouteParam";
    public const string AggregateRouteParam = "aggregateRouteParam";
    public const string ActionRouteParam = "actionRouteParam";
    public const string ActionFormat = "api/{DomainName}/{AggregateName}/{AggregateId}/{ActionName}";
    public const string SingletonActionFormat = "api/{DomainName}/{AggregateName}/{ActionName}";

    public class Params
    {
      public readonly string AggregateId = "{aggregateId}";

      public static Params ForController { get; } = new Params { DomainName = "{domainRouteParam}", AggregateName = "{aggregateRouteParam}", ActionName = "{actionRouteParam}" };

      public string DomainName { get; set; }

      public string AggregateName { get; set; }

      public string ActionName { get; set; }
    }
  }
}
