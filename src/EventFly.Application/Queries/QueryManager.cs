using System;
using System.Security.Cryptography;
using System.Text;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using EventFly.Exceptions;
using Newtonsoft.Json;

namespace EventFly.Queries
{
    public class QueryManager<TQueryHandler, TQuery, TResult> : ReceiveActor
        where TQueryHandler : ActorBase, IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        protected ILoggingAdapter Logger { get; set; }

        public string Name { get; }

        private readonly string _contextName;

        public QueryManager(string contextName)
        {
            _contextName = contextName;
            Logger = Context.GetLogger();
            Name = GetType().PrettyPrint();

            Receive<Terminated>(Terminate);

            Receive<TQuery>(Dispatch);
             
        }

        protected virtual bool Dispatch(TQuery query)
        {
            Logger.Info("QueryManager of Type={0}; has received a query of Type={1}", Name, query.GetType().PrettyPrint());

            var queryId = GenerateQueryId(query);

            var aggregateRef = FindOrCreate(queryId);

            aggregateRef.Forward(query);

            return true;
        }


        protected virtual bool ReDispatch(TQuery query)
        {
            Logger.Info("QueryManager of Type={0}; is ReDispatching deadletter of Type={1}", Name, query.GetType().PrettyPrint());

            var queryId = GenerateQueryId(query);
            var aggregateRef = FindOrCreate(queryId);

            aggregateRef.Forward(query);

            return true;
        }

        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
 
            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        private string GenerateQueryId(TQuery query)
        {
            var json = JsonConvert.SerializeObject(query);
            return _contextName + CalculateMD5Hash(json);
        }


        protected virtual bool Terminate(Terminated message)
        {
            Logger.Warning("Query of Type={0}, and Id={1}; has terminated.",typeof(TQuery).PrettyPrint(), message.ActorRef.Path.Name);
            Context.Unwatch(message.ActorRef);
            return true;
        }

        protected virtual IActorRef FindOrCreate(string queryId)
        {
            var aggregate = Context.Child(queryId);

            if(aggregate.IsNobody())
            {
                aggregate = CreateQueryHandler(queryId);
            }

            return aggregate;
        }

        protected virtual IActorRef CreateQueryHandler(string queryId)
        {
            Props props;
            try
            {
                props = Context.DI().Props<TQueryHandler>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "No DI available at the moment, falling back to default props creation.");
                props = Props.Create<TQueryHandler>();
            }

            var aggregateRef = Context.ActorOf(props, queryId);
            Context.Watch(aggregateRef);
            return aggregateRef;
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            var logger = Logger;
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeMilliseconds: 3000,
                localOnlyDecider: x =>
                {

                    logger.Warning("QueryManager of Type={0}; will supervise Exception={1} to be decided as {2}.",Name, x.ToString(), Directive.Restart);
                    return Directive.Restart;
                });
        }

    }
}