using System;

namespace EventFly.ReadModels
{
    public interface IReadModelStorage
    {

    }
    public interface IReadModelStorage<TReadModel> : IDisposable, IReadModelStorage
        where TReadModel : IReadModel
    {
        TReadModel Load(String id);
        void Save(String id, TReadModel model);
        void PreApply(TReadModel model);
    }
}