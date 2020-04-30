using EventFly.ReadModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EventFly.Storages.EntityFramework
{
    public class EntityFrameworkReadModelStorage<TReadModel, TDbContext> : IQueryableReadModelStorage<TReadModel>
        where TReadModel : ReadModel, new()
        where TDbContext : DbContext, new()
    {
        private TDbContext _dbContext;

        public EntityFrameworkReadModelStorage()
        {
            _dbContext = new TDbContext();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _dbContext = null;
        }

        public TReadModel Load(String id)
        {
            var item = _dbContext.Find<TReadModel>(id);
            if (item == null)
            {
                item = new TReadModel { Id = id };
                _dbContext.Add(item);
            }

            return item;
        }

        public void Save(String id, TReadModel model)
        {
            if (_dbContext.Entry(model).State == EntityState.Detached)
            {
                throw new InvalidOperationException("Aggregate State not attached to context.");
            }
            _dbContext.SaveChanges();
        }

        public void PreApply(TReadModel model)
        {
        }

        public IQueryable<TReadModel> Items => _dbContext.Set<TReadModel>();
    }
}