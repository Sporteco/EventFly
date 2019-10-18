using Akka.Actor;
using EventFly.Aggregates;

namespace EventFly.ReadModels
{
    public class AggregateReadModelManager<TReadModel, TIdentity> : ReadModelManager<TReadModel, TIdentity> 
        where TReadModel : ActorBase, IReadModel<TIdentity>
    {
        protected override string GetReadModelId(IDomainEvent domainEvent)
        {
            return domainEvent.GetIdentity().Value;
        }
    }
}