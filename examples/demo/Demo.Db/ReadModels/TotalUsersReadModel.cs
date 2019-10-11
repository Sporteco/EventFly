using Akkatecture.Aggregates;
using Akkatecture.ReadModels;
using Demo.Events;

namespace Demo.Db.ReadModels
{
    public class TotalUsersReadModel : ReadModel<string>,
    IAmReadModelFor<UserId, UserCreatedEvent>
    {
        public int UsersCount { get; private set; }

        public void Apply(IReadModelContext context, IDomainEvent<UserId, UserCreatedEvent> domainEvent)
        {
            Id = context.ReadModelId;
            UsersCount++;
        }
    }

    public class TotalUsersReadModelManager : ReadModelManager<TotalUsersReadModel, string>
    {
        protected override string GetReadModelId(IDomainEvent domainEvent) => "Total";
    }
}
