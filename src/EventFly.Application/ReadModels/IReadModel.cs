using EventFly.Aggregates;

namespace EventFly.ReadModels
{
    public interface IReadModel
    {
        string Id { get;  }
        void ApplyEvent(IDomainEvent e);
    }
}