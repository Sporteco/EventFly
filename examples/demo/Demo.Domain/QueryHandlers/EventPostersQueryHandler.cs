using EventFly.Queries;
using Demo.Queries;
using System.Threading.Tasks;

namespace Demo.Domain.QueryHandlers
{
    public class EventPostersQueryHandler : QueryHandler<EventPostersQuery, EventPosters>
    {
        public override Task<EventPosters> ExecuteQuery(EventPostersQuery query)
        {
            return Task.FromResult(new EventPosters());
        }
    }
}