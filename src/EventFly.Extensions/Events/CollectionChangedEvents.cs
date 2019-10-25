using System;
using System.Collections.Generic;
using System.Linq;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.Core;
using EventFly.Entities;
using EventFly.ValueObjects;

namespace EventFly.Events
{
    public abstract class ValueObjectCollectionChangedEvent<TIdentity, TCollectionItem> : AggregateEvent<TIdentity>
        where TIdentity : IIdentity
        where TCollectionItem : ValueObject
    {
        public ICollection<TCollectionItem> AddOrUpdate { get; }
        public ICollection<TCollectionItem> Remove { get; }

        protected ValueObjectCollectionChangedEvent(ChangeValueObjectCollectionCommand<TIdentity, TCollectionItem> cmd)
        {
            AddOrUpdate = cmd.AddOrUpdate;
            Remove = cmd.Remove;
        }
        public void ApplyChanges(ICollection<TCollectionItem> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (AddOrUpdate != null && AddOrUpdate.Any())
                foreach (var item in AddOrUpdate)
                {
                    if (!collection.Contains(item))
                        collection.Add(item);
                }
            if (Remove != null && Remove.Any())
                foreach (var item in Remove)
                {
                    if (!collection.Contains(item))
                        collection.Remove(item);
                }
        }
    }

    public abstract class EntityCollectionChangedEvent<TIdentity, TEntity, TEntityIdentity> : AggregateEvent<TIdentity>
        where TIdentity : IIdentity
        where TEntityIdentity : IIdentity
        where TEntity : Entity<TEntityIdentity>
    {
        public ICollection<TEntity> AddOrUpdate { get; }
        public ICollection<TEntityIdentity> Remove { get; }

        protected EntityCollectionChangedEvent(ChangeEntityCollectionCommand<TIdentity, TEntity,TEntityIdentity> cmd)
        {
            AddOrUpdate = cmd.AddOrUpdate;
            Remove = cmd.Remove;
        }
        public void ApplyChanges(ICollection<TEntity> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            var toRemove = new List<TEntityIdentity>();
            if (Remove != null && Remove.Any())
                toRemove.AddRange(Remove);

            if (AddOrUpdate != null && AddOrUpdate.Any())
                toRemove.AddRange(AddOrUpdate.Select(i=>i.Id));

            foreach (var item in collection.Where(i=>toRemove.Contains(i.Id)).ToList())
            {
                collection.Remove(item);
            }
            if (AddOrUpdate != null)
                ((List<TEntity>)collection).AddRange(AddOrUpdate);
        }
    }

    public abstract class ValueObjectWithCodeCollectionChangedEvent<TIdentity, TValueObjectWithCode, TCode> : AggregateEvent<TIdentity>
        where TIdentity : IIdentity
        where TCode : SingleValueObject<string>
        where TValueObjectWithCode : ValueObjectWithCode<TCode>
    {
        public ICollection<TValueObjectWithCode> AddOrUpdate { get; }
        public ICollection<TCode> Remove { get; }

        protected ValueObjectWithCodeCollectionChangedEvent(ChangeValueObjectWithCodeCollectionCommand<TIdentity, TValueObjectWithCode, TCode> cmd)
        {
            AddOrUpdate = cmd.AddOrUpdate;
            Remove = cmd.Remove;
        }
        public void ApplyChanges(ICollection<TValueObjectWithCode> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            var toRemove = new List<TCode>();
            if (Remove != null && Remove.Any())
                toRemove.AddRange(Remove);

            if (AddOrUpdate != null && AddOrUpdate.Any())
                toRemove.AddRange(AddOrUpdate.Select(i=>i.Code));

            foreach (var item in collection.Where(i=>toRemove.Contains(i.Code)).ToList())
            {
                collection.Remove(item);
            }
            if (AddOrUpdate != null)
                ((List<TValueObjectWithCode>)collection).AddRange(AddOrUpdate);
        }
    }
}
