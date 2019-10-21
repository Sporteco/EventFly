﻿using System;
using EventFly.Aggregates;
using EventFly.AggregateStorages;
using EventFly.Core;
using Microsoft.EntityFrameworkCore;

namespace EventFly.Storages.EntityFramework
{
    public class EntityFrameworkAggregateStorage<TAggregate, TDbContext> : IAggregateStorage<TAggregate>
        where TAggregate : IAggregateRoot
        where TDbContext : DbContext, new()
    {
        private TDbContext _dbContext;

        public EntityFrameworkAggregateStorage()
        {
            _dbContext = new TDbContext();
        }
        public void SaveState<TAggregateState, TIdentity>(TAggregateState state) where TAggregateState : IAggregateState<TIdentity> where TIdentity : IIdentity
        {
            if (_dbContext.Entry(state).State == EntityState.Detached)
            {
                throw new InvalidOperationException("Aggregate State not attached to context.");
            }
            _dbContext.SaveChanges();
        }

        public TAggregateState LoadState<TAggregateState, TIdentity>(string id) where TAggregateState : class, IAggregateState<TIdentity>, new() where TIdentity : IIdentity
        {
            var item = _dbContext.Find<TAggregateState>(id);
            if (item == null)
            {
                item = new TAggregateState{Id = id};
                _dbContext.Add(item);
            }

            return item;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _dbContext = null;
        }
    }
}