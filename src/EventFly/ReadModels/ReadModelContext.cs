namespace EventFly.ReadModels
{
    internal class ReadModelContext : IReadModelContext
    {
        public ReadModelContext(string readModelId)
        {
            ReadModelId = readModelId;
        }

        public bool IsMarkedForDeletion { get; private set; }
        public string ReadModelId { get; }
        public void MarkForDeletion()
        {
            IsMarkedForDeletion = true;
        }
    }
}