using Akkatecture.Queries;
using Demo.Queries;

namespace Demo.Domain.QueryHandlers
{
    public class EventPostersQueryHandler : QueryHandler<EventPostersQuery, EventPosters>
    {
        public override EventPosters ExecuteQuery(EventPostersQuery query)
        {
            throw new System.NotImplementedException();
        }
    }
}