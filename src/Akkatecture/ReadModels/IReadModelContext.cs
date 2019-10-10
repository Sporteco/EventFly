namespace Akkatecture.ReadModels
{
    public interface IReadModelContext
    {
        bool IsMarkedForDeletion { get; }
        string ReadModelId { get; }
        void MarkForDeletion();
    }
}