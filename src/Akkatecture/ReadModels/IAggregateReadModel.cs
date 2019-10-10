namespace Akkatecture.ReadModels
{
    public interface IAggregateReadModel : IReadModel
    {
        string Id { get; }
        long Version { get; }
    }
}