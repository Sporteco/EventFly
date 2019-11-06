using System;
using EventFly.Aggregates;
using EventFly.Core;
using Microsoft.EntityFrameworkCore;

namespace EventFly.Storages.EntityFramework
{
    public abstract class EntityFrameworkAggregateState<TAggregate, TIdentity, TDbContext> : AggregateState<TAggregate, TIdentity>, IDisposable
        where TAggregate : IAggregateRoot<TIdentity> where TIdentity : IIdentity
        where TDbContext : DbContext, new()
    {
        private TDbContext _dbContext;
        protected TDbContext DbContext => _dbContext;

        protected EntityFrameworkAggregateState()
        {
            _dbContext = new TDbContext();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _dbContext = null;
        }

    }
}