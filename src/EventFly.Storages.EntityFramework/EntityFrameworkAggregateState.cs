using EventFly.Aggregates;
using EventFly.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EventFly.Storages.EntityFramework
{
    public abstract class EntityFrameworkAggregateState<TAggregateState, TIdentity, TDbContext> : AggregateState<TAggregateState, TIdentity>, IDisposable
        where TDbContext : DbContext, new() where TAggregateState : class, IAggregateState<TIdentity>, IMessageApplier<TIdentity>
        where TIdentity : IIdentity
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

        protected override async Task PostApplyAction(IAggregateEvent<TIdentity> @event)
        {
            await DbContext.SaveChangesAsync();

            await base.PostApplyAction(@event);
        }
    }
}