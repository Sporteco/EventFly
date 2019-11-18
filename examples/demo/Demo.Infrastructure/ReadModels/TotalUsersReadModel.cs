using Demo.User.Events;
using EventFly.Aggregates;
using EventFly.ReadModels;

namespace Demo.Infrastructure.ReadModels
{
    public class TotalUsersReadModel : ReadModel<TotalUsersReadModel>,
        IAmReadModelFor<UserId, UserCreatedEvent>
    {
        public System.Int32 UsersCount { get; private set; }

        public void Apply(IDomainEvent<UserId, UserCreatedEvent> domainEvent)
        {
            UsersCount++;
        }
    }

    public class TotalUsersReadModelManager : ReadModelManager<TotalUsersReadModel>
    {
        protected override System.String GetReadModelId(IDomainEvent domainEvent) => "Total";
    }
}
