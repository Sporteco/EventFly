using EventFly.Aggregates;

namespace EventFly.ReadModels
{
    public class AggregateReadModelManager<TReadModel, TIdentity> : ReadModelManager<TReadModel> 
        where TReadModel : ReadModel, new()
    {

        protected override string GetReadModelId(IDomainEvent domainEvent)
        {
            return domainEvent.GetIdentity().Value;
        }
    }
}