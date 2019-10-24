using System;

namespace EventFly.ReadModels
{
    public interface IReadModelStorage
    {

    }
    public interface IReadModelStorage<TReadModel> : IDisposable, IReadModelStorage
        where TReadModel : IReadModel
    {
        TReadModel Load(string id);
        void  Save(string id, TReadModel model);
        void PreApply(TReadModel model);
    }
}