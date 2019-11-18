using System.Collections.Concurrent;
using System.Linq;

namespace EventFly.ReadModels
{
    public class InMemoryReadModelStorage<TReadModel> : IQueryableReadModelStorage<TReadModel> where TReadModel : ReadModel, new()
    {
        protected readonly ConcurrentDictionary<System.String, TReadModel> _items = new ConcurrentDictionary<System.String, TReadModel>();
        public void Dispose() { }
        public TReadModel Load(System.String id) => _items.GetOrAdd(id, v => new TReadModel { Id = id });
        public void Save(System.String id, TReadModel model) { }

        public void PreApply(TReadModel model) { }
        public IQueryable<TReadModel> Items => _items.Values.AsQueryable();
    }
}