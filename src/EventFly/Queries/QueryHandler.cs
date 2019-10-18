using Akka.Actor;

namespace EventFly.Queries
{
    public abstract class QueryHandler<TQuery, TResult> : ReceiveActor, IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        protected QueryHandler()
        {
            Receive<TQuery>(query =>
            {
                var result = ExecuteQuery(query);
                Context.Sender.Tell(result);
            });
        }

        public abstract TResult ExecuteQuery(TQuery query);
    }
}