using EventFly.Aggregates;
using EventFly.Exceptions;
using EventFly.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventFly.ReadModels
{
    public abstract class ReadModel : IReadModel
    {
        public String Id { get; set; }
        public abstract void ApplyEvent(IDomainEvent e);
        internal IReadModelStorage Storage { get; set; }

    }
    public abstract class ReadModel<TReadModel> : ReadModel
    where TReadModel : ReadModel<TReadModel>
    {
        private static readonly IReadOnlyDictionary<Type, Action<TReadModel, IDomainEvent>> ApplyMethods = typeof(TReadModel).GetReadModelEventApplyMethods<TReadModel>();

        public override void ApplyEvent(IDomainEvent e)
        {
            var applyMethods = GetEventApplyMethods(e);
            applyMethods(e);

        }
        protected Action<IDomainEvent> GetEventApplyMethods(IDomainEvent aggregateEvent)
        {
            var eventType = aggregateEvent.GetType();
            var method = ApplyMethods.FirstOrDefault(i => i.Key.IsAssignableFrom(eventType)).Value;

            if (method == null)
                throw new NotImplementedException($"ReadModel of Type={GetType().PrettyPrint()} does not have an 'Apply' method that takes in an aggregate event of Type={eventType.PrettyPrint()} as an argument.");

            var aggregateApplyMethod = method.Bind((TReadModel)this);

            return aggregateApplyMethod;
        }
    }
}