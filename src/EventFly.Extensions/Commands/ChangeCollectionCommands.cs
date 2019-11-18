using EventFly.Core;
using EventFly.Entities;
using EventFly.ValueObjects;
using System.Collections.Generic;

namespace EventFly.Commands
{
    public abstract class ChangeValueObjectCollectionCommand<TIdentity, TCollectionItem> : Command<TIdentity>
        where TIdentity : IIdentity
        where TCollectionItem : ValueObject
    {
        public ICollection<TCollectionItem> AddOrUpdate { get; } = new List<TCollectionItem>();
        public ICollection<TCollectionItem> Remove { get; } = new List<TCollectionItem>();
        protected ChangeValueObjectCollectionCommand(TIdentity aggregateId, CommandMetadata metadata = null) : base(aggregateId, metadata) { }
    }

    public abstract class ChangeEntityCollectionCommand<TIdentity, TEntity, TEntityIdentity> : Command<TIdentity>
        where TIdentity : IIdentity
        where TEntityIdentity : IIdentity
        where TEntity : Entity<TEntityIdentity>
    {
        public ICollection<TEntity> AddOrUpdate { get; } = new List<TEntity>();
        public ICollection<TEntityIdentity> Remove { get; } = new List<TEntityIdentity>();
        protected ChangeEntityCollectionCommand(TIdentity aggregateId, CommandMetadata metadata = null) : base(aggregateId, metadata) { }
    }

    public abstract class ChangeValueObjectWithCodeCollectionCommand<TIdentity, TValueObjectWithCode, TCode> : Command<TIdentity>
        where TIdentity : IIdentity
        where TCode : SingleValueObject<System.String>
        where TValueObjectWithCode : ValueObjectWithCode<TCode>
    {
        public ICollection<TValueObjectWithCode> AddOrUpdate { get; } = new List<TValueObjectWithCode>();
        public ICollection<TCode> Remove { get; } = new List<TCode>();
        protected ChangeValueObjectWithCodeCollectionCommand(TIdentity aggregateId, CommandMetadata metadata = null) : base(aggregateId, metadata) { }
    }
}
