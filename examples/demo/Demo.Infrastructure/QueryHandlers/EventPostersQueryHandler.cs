using Demo.Queries;
using EventFly.Queries;
using System.Threading.Tasks;

namespace Demo.Infrastructure.QueryHandlers
{
    public class EventPostersQueryHandler : QueryHandler<EventPostersQuery, EventPosters>
    {
        public override Task<EventPosters> ExecuteQuery(EventPostersQuery query)
        {
            return Task.FromResult(new EventPosters());
        }
    }
}