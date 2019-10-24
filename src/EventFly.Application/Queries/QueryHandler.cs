using Akka.Actor;
using System.Threading.Tasks;

namespace EventFly.Queries
{
    public abstract class QueryHandler<TQuery, TResult> : ReceiveActor, IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        protected QueryHandler()
        {
            ReceiveAsync<TQuery>(async query =>
            {
                var result = await ExecuteQuery(query);
                Context.Sender.Tell(result);
            });
        }

        public abstract Task<TResult> ExecuteQuery(TQuery query);
    }
}