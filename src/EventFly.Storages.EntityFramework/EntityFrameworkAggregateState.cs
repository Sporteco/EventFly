using EventFly.Aggregates;
using EventFly.Core;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventFly.Storages.EntityFramework
{
    public abstract class EntityFrameworkAggregateState<TAggregate, TIdentity, TDbContext> : AggregateState<TAggregate, TIdentity>, IDisposable
        where TAggregate : IAggregateRoot<TIdentity> where TIdentity : IIdentity
        where TDbContext : DbContext, new()
    {
        public void Dispose()
        {
            DbContext.Dispose();
            DbContext = null;
        }

        protected TDbContext DbContext;

        protected EntityFrameworkAggregateState(TDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}