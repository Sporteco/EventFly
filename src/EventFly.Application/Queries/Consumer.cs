using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using EventFly.Aggregates;
using EventFly.Events;
using EventFly.Extensions;
using System.Linq;

namespace EventFly.Queries
{



    public class Consumer
    {
        public ActorSystem ActorSystem { get; set; }
        protected System.String Name { get; set; }
        internal Consumer(
            System.String name,
            ActorSystem actorSystem)
        {
            ActorSystem = actorSystem;
            Name = name;
        }

        private Consumer(
            System.String name,
            Config config)
        {
            var actorSystem = ActorSystem.Create(name, config);
            ActorSystem = actorSystem;
            Name = name;
        }

        public static Consumer Create(System.String name, Config config)
        {
            return new Consumer(name, config);
        }

        public static Consumer Create(ActorSystem actorSystem)
        {
            return new Consumer(actorSystem.Name, actorSystem);
        }

        public Consumer<TJournal> Using<TJournal>(
            System.String readJournalPluginId = null)
            where TJournal : IEventsByTagQuery, ICurrentEventsByTagQuery
        {
            var readJournal = PersistenceQuery
                .Get(ActorSystem)
                .ReadJournalFor<TJournal>(readJournalPluginId);

            return new Consumer<TJournal>(Name, ActorSystem, readJournal);
        }
    }

    public class Consumer<TJournal> : Consumer
        where TJournal : IEventsByTagQuery, ICurrentEventsByTagQuery
    {
        protected TJournal Journal { get; }

        public Consumer(
            System.String name,
            ActorSystem actorSystem,
            TJournal journal)
            : base(name, actorSystem)
        {
            Journal = journal;
        }

        public Source<EventEnvelope, NotUsed> EventsFromAggregate<TAggregate>(Offset offset = null)
            where TAggregate : IAggregateRoot
        {
            var mapper = new DomainEventReadAdapter();
            var aggregateName = typeof(TAggregate).GetAggregateName();

            return Journal
                .EventsByTag(aggregateName.Value, offset)
                .Select(x =>
                {
                    var domainEvent = mapper.FromJournal(x.Event, System.String.Empty).Events.Single();
                    return new EventEnvelope(x.Offset, x.PersistenceId, x.SequenceNr, domainEvent);
                });
        }

        public Source<EventEnvelope, NotUsed> CurrentEventsFromAggregate<TAggregate>(Offset offset = null)
            where TAggregate : IAggregateRoot
        {
            var mapper = new DomainEventReadAdapter();
            var aggregateName = typeof(TAggregate).GetAggregateName();

            return Journal
                .EventsByTag(aggregateName.Value, offset)
                .Select(x =>
                {
                    var domainEvent = mapper.FromJournal(x.Event, System.String.Empty).Events.Single();
                    return new EventEnvelope(x.Offset, x.PersistenceId, x.SequenceNr, domainEvent);
                });
        }
    }


}