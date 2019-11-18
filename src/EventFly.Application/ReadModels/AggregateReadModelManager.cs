using EventFly.Aggregates;

namespace EventFly.ReadModels
{
    public class AggregateReadModelManager<TReadModel, TIdentity> : ReadModelManager<TReadModel>
        where TReadModel : ReadModel, new()
    {
        protected override System.String GetReadModelId(IDomainEvent domainEvent)
        {
            return domainEvent.GetIdentity().Value;
        }
    }
}