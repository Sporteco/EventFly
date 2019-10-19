using System.Linq;

namespace EventFly.ReadModels
{
    public interface IQueryableReadModelStorage<TReadModel> : IReadModelStorage<TReadModel>
        where TReadModel : IReadModel
    {
        IQueryable<TReadModel> Items { get; }
    }
}