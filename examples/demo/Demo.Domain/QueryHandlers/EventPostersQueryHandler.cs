using EventFly.Queries;
using Demo.Queries;

namespace Demo.Domain.QueryHandlers
{
    public class EventPostersQueryHandler : QueryHandler<EventPostersQuery, EventPosters>
    {
        public override EventPosters ExecuteQuery(EventPostersQuery query)
        {
            return new EventPosters();
        }
    }
}