namespace Akkatecture.ReadModels
{
    public interface IReadModel
    {
    }

    public interface IReadModel<out TKey> : IReadModel
    {
        TKey Id { get; }
    }
}