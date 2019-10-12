using System;
using System.Collections.Generic;

namespace Akkatecture.Meta
{
    public interface IDomainDefinition
    {
        string Name { get; }
        IReadOnlyCollection<IAggregateDefinition> Aggregates { get; }
    }
    public abstract class DomainDefinition : IDomainDefinition
    {
        public string Name { get; }

        public IReadOnlyCollection<IAggregateDefinition> Aggregates { get; }

        protected DomainDefinition(IReadOnlyCollection<IAggregateDefinition> aggregates)
        {
            Name = GetType().Name;
            Aggregates = aggregates;
        }
    }
}