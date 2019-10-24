using EventFly.Aggregates;
using EventFly.ReadModels;
using Demo.Events;

namespace Demo.Infrastructure.ReadModels
{
    public class TotalUsersReadModel : ReadModel<TotalUsersReadModel>,
        IAmReadModelFor<UserId, UserCreatedEvent>
    {
        public int UsersCount { get; private set; }

        public void Apply(IDomainEvent<UserId, UserCreatedEvent> domainEvent)
        {
            UsersCount++;
        }
    }

    public class TotalUsersReadModelManager : ReadModelManager<TotalUsersReadModel>
    {
        protected override string GetReadModelId(IDomainEvent domainEvent) => "Total";
    }
}
