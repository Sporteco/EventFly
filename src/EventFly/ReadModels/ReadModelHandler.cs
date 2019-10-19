using System.Reflection;
using Akka.Actor;
using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Extensions;

namespace EventFly.ReadModels
{
    public class ReadModelHandler<TReadModel> : ReceiveActor, IReadModelHandler
        where TReadModel : ReadModel, new()
    {
        private readonly IReadModelStorage<TReadModel> _storage;
        private TReadModel ReadModel { get; set; }
        public ReadModelHandler(IReadModelStorage<TReadModel> storage)
        {
            _storage = storage;

            var subscriptionTypes = typeof(TReadModel).GetReadModelSubscribersTypes();
            foreach (var types in subscriptionTypes)
            {
                var method = GetType().GetMethod(nameof(Command), BindingFlags.Instance | BindingFlags.NonPublic);
                method?.MakeGenericMethod(types.Item1, types.Item2).Invoke(this, null);
            }
        }

        protected override void PreStart()
        {
            base.PreStart();
            ReadModel = _storage.Load(Self.Path.Name);
        }

        protected override void PostStop()
        {
            base.PostStop();
            _storage.Dispose();
        }

        private void Command<TIdentity, TAggregateEvent>()
            where TIdentity : IIdentity
            where TAggregateEvent : class, IAggregateEvent<TIdentity>
        {
            Receive<IDomainEvent<TIdentity, TAggregateEvent>>(e =>
            {
                _storage.PreApply(ReadModel);
                ReadModel.ApplyEvent(e);
                _storage.Save(Self.Path.Name, ReadModel);
            });
        }

    }
}