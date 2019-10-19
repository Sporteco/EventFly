using System.Collections.Concurrent;
using System.Linq;

namespace EventFly.ReadModels
{
    public class InMemoryReadModelStorage<TReadModel> : IQueryableReadModelStorage<TReadModel> where TReadModel : ReadModel, new()
    {
        protected readonly ConcurrentDictionary<string,TReadModel> _items = new ConcurrentDictionary<string,TReadModel>();
        public void Dispose(){}
        public TReadModel Load(string id) => _items.GetOrAdd(id, v => new TReadModel{Id = id});
        public void Save(string id, TReadModel model){}

        public void PreApply(TReadModel model){}
        public IQueryable<TReadModel> Items => _items.Values.AsQueryable();
    }
}